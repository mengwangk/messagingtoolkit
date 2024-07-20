// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    // Barcode types
    var barcodeTypes = new Array(
            MessagingToolkit.Barcode.BarcodeFormat.qrCode,
            MessagingToolkit.Barcode.BarcodeFormat.dataMatrix,
            MessagingToolkit.Barcode.BarcodeFormat.pdf417,
            MessagingToolkit.Barcode.BarcodeFormat.codabar,
            MessagingToolkit.Barcode.BarcodeFormat.code128,
            MessagingToolkit.Barcode.BarcodeFormat.code39,
            MessagingToolkit.Barcode.BarcodeFormat.ean13,
            MessagingToolkit.Barcode.BarcodeFormat.ean8,
            MessagingToolkit.Barcode.BarcodeFormat.itf14,
            MessagingToolkit.Barcode.BarcodeFormat.upca,
            MessagingToolkit.Barcode.BarcodeFormat.msimod10
        );

    // Character set
    var barcodeCharacterSets = new Array(
            "ISO-8859-1",
            "UTF-8",
            "SHIFT-JIS",
            "CP437"
        );


    // Error correction level
    var barcodeECLs = new Array(
           MessagingToolkit.Barcode.QRCode.Decoder.ErrorCorrectionLevel.l,
           MessagingToolkit.Barcode.QRCode.Decoder.ErrorCorrectionLevel.m,
           MessagingToolkit.Barcode.QRCode.Decoder.ErrorCorrectionLevel.q,
           MessagingToolkit.Barcode.QRCode.Decoder.ErrorCorrectionLevel.h
       );


    // Generated barcode stream
    var barcodeStream = null;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());

            // Retrieve the button and register our event handler. 
            var showButton = document.getElementById("btnShow");
            showButton.addEventListener("click", showButtonClickHandler, false);

            var generateButton = document.getElementById("btnGenerateBarcode");
            generateButton.addEventListener("click", generateButtonClickHandler, false);

            var saveButton = document.getElementById("btnSaveBarcode");
            saveButton.addEventListener("click", saveButtonClickHandler, false);

            var generateEpsButton = document.getElementById("btnGenerateEPS");
            generateEpsButton.addEventListener("click", generateEpsButtonClickHandler, false);

            var generateSvgButton = document.getElementById("btnGenerateSVG");
            generateSvgButton.addEventListener("click", generateSvgButtonClickHandler, false);

            var browseButton = document.getElementById("btnBrowse");
            browseButton.addEventListener("click", browseButtonClickHandler, false);
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };


    function showButtonClickHandler(eventInfo) {
        var encoder = document.getElementById("encoder");
        var decoder = document.getElementById("decoder");
        var showButton = document.getElementById("btnShow");

        var currentText = showButton.innerText;
        if (currentText == 'Show Encoder') {
            encoder.setAttribute("style", "visibility: visible; display:block");
            decoder.setAttribute("style", "visibility:hidden; display:none");
            showButton.innerText = "Show Decoder";
        } else {
            encoder.setAttribute("style", "visibility:hidden; display:none");
            decoder.setAttribute("style", "visibility: visible; display:block");
            showButton.innerText = "Show Encoder";
        }
    }

    // Generate the barcode image
    function generateButtonClickHandler(eventInfo) {

        // Get content
        var content = document.getElementById("txtContent").innerText;

        // Get barcode type
        var barcodeType = barcodeTypes[document.getElementById("barcodeType").selectedIndex];

        // Get character set
        var barcodeCharacterSet = barcodeCharacterSets[document.getElementById("barcodeCharacterSet").selectedIndex];

        // Get error correction level
        var barcodeECL = barcodeECLs[document.getElementById("barcodeErrorCorrectionLevel").selectedIndex];

        // Get quiet zone
        var barcodeQuietZone = document.getElementById("barcodeQuietZone").value;

        // Get width
        var barcodeWidth = document.getElementById("barcodeWidth").value;

        // Get height
        var barcodeHeight = document.getElementById("barcodeHeight").value;

        var barcodeEncoder = new MessagingToolkit.Barcode.BarcodeEncoder();
        barcodeEncoder.margin = barcodeQuietZone;
        barcodeEncoder.characterSet = barcodeCharacterSet;
        barcodeEncoder.width = barcodeWidth;
        barcodeEncoder.height = barcodeHeight;
        barcodeEncoder.errorCorrectionLevel = barcodeECL;

        try {
            barcodeEncoder.encode(barcodeType, content).then(
                function (stream) {
                    barcodeStream = stream;
                    var rasr = Windows.Storage.Streams.RandomAccessStreamReference.createFromStream(stream);
                    rasr.openReadAsync().then(function (iraswct) {
                        // iraswct is an IRandomAccessStreamWithContentType
                        var barcodeImage = document.getElementById("imgGeneratedBarcode");
                        barcodeImage.src = URL.createObjectURL(iraswct, { oneTimeOnly: true });
                    });
                    return Windows.Graphics.Imaging.BitmapDecoder.createAsync(stream);
                }, function (error) {
                    var errorOutput = document.getElementById("status");
                    errorOutput.innerText = error.message;
                    Windows.UI.Popups.MessageDialog(error.message);
                }
            ).done(
               function (decoder) {
                   if (decoder) {
                       // No action 
                   }
               }
            ), function (error) {
                var errorOutput = document.getElementById("status");
                errorOutput.innerText = error.message;
                Windows.UI.Popups.MessageDialog(error.message);
            };
        } catch (err) {
            var errorOutput = document.getElementById("status");
            errorOutput.innerText = err.message;
            Windows.UI.Popups.MessageDialog(err.message);
        }
    }

    function saveButtonClickHandler(eventInfo) {

        if (barcodeStream == null) return;  // Nothing to save

        // Create the picker object and set options 
        var savePicker = new Windows.Storage.Pickers.FileSavePicker();
        savePicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
        // Dropdown of file types the user can save the file as 
        savePicker.fileTypeChoices.insert("PNG", [".png"]);
        savePicker.suggestedFileName = "barcode";
        savePicker.pickSaveFileAsync().done(
            function (file) {
                if (file) {
                    Windows.Storage.CachedFileManager.deferUpdates(file);
                    var inputStream = barcodeStream.getInputStreamAt(0);
                    var dataReader = new Windows.Storage.Streams.DataReader(inputStream);
                    dataReader.loadAsync(barcodeStream.size).done(
                        function () {
                            var buffer = new Uint8Array(barcodeStream.size);
                            dataReader.readBytes(buffer);
                            Windows.Storage.FileIO.writeBytesAsync(file, buffer).done(
                       function () {
                           Windows.Storage.CachedFileManager.completeUpdatesAsync(file).done(
                               function (updateStatus) {
                                   if (updateStatus === Windows.Storage.Provider.FileUpdateStatus.complete) {
                                       var status = document.getElementById("status");
                                       status.innerText = "File is saved successfully";
                                   } else {
                                       // ... 
                                   }
                               });
                       });
                        }
                    );
                } else {
                    // Picker was dismissed
                }

            });
    }

    function generateEpsButtonClickHandler(eventInfo) {

        // Get content
        var content = document.getElementById("txtContent").innerText;

        // Get barcode type
        var barcodeType = barcodeTypes[document.getElementById("barcodeType").selectedIndex];

        // Get character set
        var barcodeCharacterSet = barcodeCharacterSets[document.getElementById("barcodeCharacterSet").selectedIndex];

        // Get error correction level
        var barcodeECL = barcodeECLs[document.getElementById("barcodeErrorCorrectionLevel").selectedIndex];

        // Get quiet zone
        var barcodeQuietZone = document.getElementById("barcodeQuietZone").value;

        // Get width
        var barcodeWidth = document.getElementById("barcodeWidth").value;

        // Get height
        var barcodeHeight = document.getElementById("barcodeHeight").value;

        var barcodeEncoder = new MessagingToolkit.Barcode.BarcodeEncoder();
        barcodeEncoder.margin = barcodeQuietZone;
        barcodeEncoder.characterSet = barcodeCharacterSet;
        barcodeEncoder.width = barcodeWidth;
        barcodeEncoder.height = barcodeHeight;
        barcodeEncoder.errorCorrectionLevel = barcodeECL;

        try {
            var provider = new MessagingToolkit.Barcode.Provider.EpsProvider();
            var output = barcodeEncoder.generate(barcodeType, content, provider);

            var fileName = "barcode.eps";
            var folder = Windows.Storage.KnownFolders.picturesLibrary;
            var option = Windows.Storage.CreationCollisionOption.replaceExisting;

            folder.createFileAsync(fileName, option).done(
            function (file) {
                if (file) {
                    Windows.Storage.FileIO.writeTextAsync(file, output.content).done(
                        function () {
                            var status = document.getElementById("status");
                            status.innerText = fileName + " is saved successfully to " + folder.displayName + " folder";
                        });
                }
            });

        } catch (err) {
            var errorOutput = document.getElementById("status");
            errorOutput.innerText = err.message;
            Windows.UI.Popups.MessageDialog(err.message);
        }
    }

    function generateSvgButtonClickHandler(eventInfo) {
        // Get content
        var content = document.getElementById("txtContent").innerText;

        // Get barcode type
        var barcodeType = barcodeTypes[document.getElementById("barcodeType").selectedIndex];

        // Get character set
        var barcodeCharacterSet = barcodeCharacterSets[document.getElementById("barcodeCharacterSet").selectedIndex];

        // Get error correction level
        var barcodeECL = barcodeECLs[document.getElementById("barcodeErrorCorrectionLevel").selectedIndex];

        // Get quiet zone
        var barcodeQuietZone = document.getElementById("barcodeQuietZone").value;

        // Get width
        var barcodeWidth = document.getElementById("barcodeWidth").value;

        // Get height
        var barcodeHeight = document.getElementById("barcodeHeight").value;

        var barcodeEncoder = new MessagingToolkit.Barcode.BarcodeEncoder();
        barcodeEncoder.margin = barcodeQuietZone;
        barcodeEncoder.characterSet = barcodeCharacterSet;
        barcodeEncoder.width = barcodeWidth;
        barcodeEncoder.height = barcodeHeight;
        barcodeEncoder.errorCorrectionLevel = barcodeECL;

        try {
            var provider = new MessagingToolkit.Barcode.Provider.SvgProvider();
            var output = barcodeEncoder.generate(barcodeType, content, provider);

            var fileName = "barcode.svg";
            var folder = Windows.Storage.KnownFolders.picturesLibrary;
            var option = Windows.Storage.CreationCollisionOption.replaceExisting;

            folder.createFileAsync(fileName, option).done(
            function (file) {
                if (file) {
                    Windows.Storage.FileIO.writeTextAsync(file, output.content).done(
                        function () {
                            var status = document.getElementById("status");
                            status.innerText = fileName + " is saved successfully to " + folder.displayName + " folder";
                        });
                }
            });
        } catch (err) {
            var errorOutput = document.getElementById("status");
            errorOutput.innerText = err.message;
            Windows.UI.Popups.MessageDialog(err.message);
        }
    }


    function browseButtonClickHandler(eventInfo) {
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.viewMode = Windows.Storage.Pickers.PickerViewMode.thumbnail;
        openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
        openPicker.fileTypeFilter.replaceAll([".png", ".jpg", ".jpeg"]);
        openPicker.pickSingleFileAsync().then(
            function (file) {
                if (file) {
                    var barcodeImage = document.getElementById("barcodeImageFile");
                    barcodeImage.src = URL.createObjectURL(file, { oneTimeOnly: true });
                    var txtFileName = document.getElementById("txtFileName");
                    txtFileName.innerText = file.path  + '\\' + file.name;
                    return file.openAsync(Windows.Storage.FileAccessMode.readWrite);
                }
            }).done(
            function (stream) {
                if (stream) {
                    var reader = new MessagingToolkit.Barcode.BarcodeDecoder();
                    reader.decode(stream).done(
                        function (result) {
                            if (result) {
                                var decodedResult = "Type: " + result.barcodeFormat +
                                                     "\r\n" + "Text: " + result.text;
                                document.getElementById("barcodeResult").innerText = decodedResult;
                            }
                        }
                        , function (error) {
                            document.getElementById("barcodeResult").innerText = error.message;
                        });
                }
            });
    }
    app.start();
})();


