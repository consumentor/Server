function onRendering(sender, args) {
    Uc.imageData = args.get_data();
    Uc.imagesList = sender;
    Sys.Observer.makeObservable(Uc.imageData);
}

function onImagesCommand(sender, args) {
    switch (args.get_commandName()) {
        // A custom command
        case "Delete":
            var imageData = Uc.imageData;
            var index = sender.get_selectedIndex();
            var deletedImage = imageData[index];

            Uc.imagesDataContext.removeEntity(deletedImage);
            imageData.remove(deletedImage);

            if (index >= imageData.length) index--;
            sender.set_selectedIndex(index);
    }
}
