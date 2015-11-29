namespace Sys.Mvc {
    using System;
    using System.DHTML;

    internal static class ValidationUtil {

        public static bool ArrayIsNullOrEmpty(object[] array) {
            return (array == null || array.Length == 0);
        }

        public static bool StringIsNullOrEmpty(string value) {
            return (value == null || value.Length == 0);
        }

        public static bool ElementSupportsEvent(DOMElement element, string eventAttributeName) {
            return (bool)Script.Literal("({0} in {1})", eventAttributeName, element);
        }

        public static void RemoveAllChildren(DOMElement element) {
            while (element.FirstChild != null) {
                element.RemoveChild(element.FirstChild);
            }
        }

        public static void SetInnerText(DOMElement element, string innerText) {
            DOMElement textNode = (DOMElement)Script.Literal("document.createTextNode({0})", innerText);
            RemoveAllChildren(element);
            element.AppendChild(textNode);
        }

    }
}
