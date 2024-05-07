document.getElementById("upload-form").addEventListener("submit", function (event) {
    event.preventDefault();

    const fileInput = document.getElementById("file-input");
    const resultDiv = document.getElementById("result");

    const file = fileInput.files[0];
    if (file) {
        if (file.type === "text/csv") {
            const reader = new FileReader();

            reader.onload = function (e) {
                const contents = e.target.result;
                const textDecoder = new TextDecoder('utf-8');
                const decodedContents = textDecoder.decode(contents);
                const dataContent = parseAndDisplay(decodedContents, file.type);
                //resultDiv.innerHTML = dataContent;
                sendDataToService(dataContent);
            };

            reader.readAsArrayBuffer(file); 
        } else {
            resultDiv.innerHTML = "Formato de archivo no válido.";
        }
    }
});

function GetKey(data) {
    var uniqueKeys = new Set();
    var data = JSON.parse(data);

    if (Array.isArray(data)) {
        data.forEach(function (obj) {
            for (var key in obj) {
                if (obj.hasOwnProperty(key)) {
                    uniqueKeys[key] = true;
                }
            }
        });

        var uniqueKeysArray = Object.keys(uniqueKeys);

        SetDiv(uniqueKeysArray);
    } else {
        console.error('El objeto "data" no es un array.');
    }
}


function parseAndDisplay(contents, fileType) {
    let parsedData = "";

    if (fileType === "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
       
    } else if (fileType === "text/csv") {
       
        const parsed = Papa.parse(contents, { header: true });
        parsedData = parsed.data;
        parsedData.forEach(removeEmptyKeys);
        parsedData = JSON.stringify(parsedData, null, 2);
    }
    GetKey(parsedData);

    return parsedData;
}

function removeEmptyKeys(obj) {
    for (var prop in obj) {
        if (obj.hasOwnProperty(prop) && prop === "") {
            delete obj[prop];
        }
    }
}

function sendDataToService(datosCsv) {
    var tipoSeleccionado = document.getElementById("seleccion-CSV");


    var datos = {
        jsonData: datosCsv,
        tipo: tipoSeleccionado.value
    }
    console.log("datos : ", datos);


    fetch("ReportesGenerales/GuardarDatosCSV", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(datos)
    })
        .then(response => response.json())
        .then(result => {
            if (result.data === "Ok") {
                Swal.fire({
                    title: "Success!!",
                    text: "Carga de archivo exitoso.",
                    icon: "success",
                    button: "Aceptar"
                });
            } else {
                Swal.fire({
                    title: "Error",
                    text: "No se pudo realizar la carga del los datos.",
                    icon: "error",
                    button: "OK",
                });
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
}


function SetDiv(uniqueKeysArray){
    var container = document.getElementById("Div-Container"); 
    container.innerHTML = "";
    uniqueKeysArray.forEach(function (key) {
        var label = document.createElement("label");
        var parrafo = document.createElement("p");
        var checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.name = key; 
        checkbox.className = "checkbox";

        parrafo.appendChild(document.createTextNode(key));
        label.appendChild(checkbox);
        label.appendChild(parrafo);

        container.appendChild(label);
    });
    localStorage.setItem('headerCSV', container.innerHTML);
    // Agrega un controlador de eventos a los checkboxes
    const checkboxes = document.querySelectorAll('.checkbox');
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            checkboxes.forEach(otherCheckbox => {
                if (otherCheckbox !== checkbox) {
                    otherCheckbox.checked = false;
                }
            });
            if (checkbox.checked) {
                localStorage.setItem('selectedCheckbox', checkbox.name);
            } else {
                localStorage.removeItem('selectedCheckbox');
            }
        });
    });

    // Restaurar el estado del checkbox desde el localStorage
    const storedValue = localStorage.getItem('selectedCheckbox');
    if (storedValue !== null) {
        checkboxes.forEach(checkbox => {
            if (checkbox.name === storedValue) {
                checkbox.checked = true;
            } else {
                checkbox.checked = false;
            }
        });
    }
}
