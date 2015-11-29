using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using Microsoft.Web.Mvc.Resources;

namespace MovieApp.Infrastructure
{
    // Handles conversion between CLR types and Atom feeds/entries
    public class AtomFormatHandler : IRequestFormatHandler, IResponseFormatHandler
    {
        bool IsAtomFormat(string mediaType)
        {
            return mediaType.StartsWith("application/atom+xml", StringComparison.OrdinalIgnoreCase);
        }

        public bool CanDeserialize(ContentType requestFormat)
        {
            return requestFormat != null && IsAtomFormat(requestFormat.MediaType);
        }

        public object Deserialize(ControllerContext controllerContext, ModelBindingContext bindingContext, ContentType requestFormat)
        {
            if (!CanDeserialize(requestFormat))
            {
                throw new NotSupportedException();
            }
            // We assume an Atom entry is being posted - since a feed is never posted in this example.
            using (XmlReader reader = XmlReader.Create(controllerContext.HttpContext.Request.InputStream, new XmlReaderSettings() { IgnoreWhitespace = true, IgnoreComments = true }))
            {
                SyndicationItem entry = SyndicationItem.Load(reader);
                XmlSyndicationContent xml = entry.Content as XmlSyndicationContent;
                if (xml == null)
                {
                    throw new InvalidOperationException("Xml content expected in Atom entry");
                }
                DataContractSerializer serializer = new DataContractSerializer(bindingContext.ModelType);
                using (XmlDictionaryReader innerReader = xml.GetReaderAtContent())
                {
                    innerReader.ReadStartElement();
                    return serializer.ReadObject(innerReader, false);
                }
            }
        }

        public bool CanSerialize(ContentType responseFormat)
        {
            return responseFormat != null && IsAtomFormat(responseFormat.MediaType);
        }

        public void Serialize(ControllerContext context, object model, ContentType responseFormat)
        {
            if (responseFormat.Parameters.ContainsKey("type"))
            {
                if (String.Equals(responseFormat.Parameters["type"], "entry"))
                {
                    // Atom entry is being requested
                    SyndicationItem entry = CreateEntryFromModel(model);
                    AtomEntryActionResult atomEntryActionResult = new AtomEntryActionResult(entry, responseFormat);
                    atomEntryActionResult.ExecuteResult(context);
                }
            }
            else
            {
                // Atom feed is being requested
                SyndicationFeed feed = CreateFeedFromModel(model);
                AtomFeedActionResult atomFeedActionResult = new AtomFeedActionResult(feed, responseFormat);
                atomFeedActionResult.ExecuteResult(context);
            }
        }

        // Creates entry from a given model
        // it assumes that the model has properties (of appropriate types) called "Id", "Title" and "LastUpdatedTime".
        SyndicationItem CreateEntryFromModel(object model)
        {
            return new SyndicationItem()
            {
                Id = GetModelProperty(model, "Id"),
                Title = new TextSyndicationContent(GetModelProperty(model, "Title")),
                LastUpdatedTime = DateTimeOffset.Parse(
                DateTime.Now.ToString()),
                Authors = { new SyndicationPerson("") },
                Content = SyndicationContent.CreateXmlContent(model)
            };
        }

        SyndicationFeed CreateFeedFromModel(object model)
        {
            IEnumerable collection = model as IEnumerable;
            if (collection == null)
            {
                var array = new ArrayList();
                array.Add(model);
                collection = array;
            }
            Collection<SyndicationItem> items = new Collection<SyndicationItem>();
            foreach (object item in collection)
            {
                items.Add(CreateEntryFromModel(item));
            }

            return new SyndicationFeed(items);
        }

        // We assume the id and last updated time can be returned as a string
        string GetModelProperty(object model, string propertyName)
        {
            return model.GetType().GetProperty(propertyName).GetValue(model, null).ToString();
        }

        public bool TryMapFormatFriendlyName(string friendlyName, out ContentType contentType)
        {
            if (string.Equals(friendlyName, this.FriendlyName, StringComparison.OrdinalIgnoreCase))
            {
                contentType = new ContentType("application/atom+xml");
                return true;
            }
            contentType = null;
            return false;
        }

        public string FriendlyName { get { return "Atom"; } }
    }
}
