/// <reference name="MicrosoftAjax.js" />
/// <reference name="MicrosoftAjaxTemplates.js" />
/// <reference name="MicrosoftAjaxDataContext.js" />
/// <reference name="MicrosoftAjaxAdoNet.js" />

var imagesList, images, dataContext;

Sys.activateDom = false; 

Sys.require([Sys.components.adoNetDataContext, Sys.components.dataView]);

Sys.onReady(function() {
    // Create DataContext pointing at Astoria Data Service
    dataContext = Sys.create.adoNetDataContext({
        serviceUri: "../Services/ImagesDataService.svc"
    });

    Sys.activateElements(document.documentElement);

    imagesList = Sys.get("$imagesListView");
    fetchData();
});
 
// Fetch images data from DataContext 
// and provide as data to Master View client control
function fetchData() {
    dataContext.fetchData("Images", null, null, null,
        function(results) {
            images = results;
            Sys.Observer.makeObservable(images);
            imagesList.set_data(images)
        }
    );
}

function saveChanges() {
    dataContext.saveChanges(
        function(results) {
            alert("Changes successfully saved to the server...");
        },
        function(error) {
            alert("Saving changes to the server was unsuccessful: " + error.get_message());
            cancelChanges();
        }
    );
}

function cancelChanges() {
    if (dataContext.get_hasChanges()) {
        fetchData(true);
        dataContext.clearChanges();
    }
}

// Add image, and set selection afterwards
function addNewImage() {
    var newImage = { Name: "Name", Description: "Description", Contributor: null, Uri: "../images/question.jpg" };
    dataContext.insertEntity(newImage, "Images");
    images.add(newImage);

    var newIndex = images.length - 1;
    imagesList.set_selectedIndex(newIndex);
    Sys.get(".listitem", imagesList.get_contexts()[newIndex]).scrollIntoView();
}

// Delete image, and set selection afterwards
function deleteImage() {
    var index = imagesList.get_selectedIndex();
    var deletedImage = images[index];

    dataContext.removeEntity(deletedImage);
    images.remove(deletedImage);

    if (index >= images.length) index--;
    imagesList.set_selectedIndex(index);
}

