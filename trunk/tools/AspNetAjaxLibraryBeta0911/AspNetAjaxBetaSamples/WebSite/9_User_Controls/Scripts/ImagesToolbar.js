function onSort(orderby) {
    Uc.fetchParameters.orderby = orderby;
    Uc.imagesList.fetchData();
}

function onInsert() {
    var newImage = { Name: "Name", Description: "Description", Contributor: "Contributor", Uri: "../images/question.jpg" };

    Uc.imagesDataContext.insertEntity(newImage);
    Uc.imageData.add(newImage);

    var newIndex = Uc.imageData.length - 1;
    Uc.imagesList.set_selectedIndex(newIndex);
    Sys.get(".listitem", Uc.imagesList.get_contexts()[newIndex]).scrollIntoView();
}
