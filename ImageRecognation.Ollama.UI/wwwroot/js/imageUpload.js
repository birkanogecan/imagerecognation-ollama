//window.imageUploadInterop = {
//    uploadImageAndShowResult: async function (inputId, apiUrl, resultDivId) {
//        const input = document.getElementById(inputId);
//        const resultDiv = document.getElementById(resultDivId);
//        if (!input || !input.files || input.files.length === 0) return;

//        const file = input.files[0];
//        const formData = new FormData();
//        formData.append("file", file, file.name);

//        const response = await fetch(apiUrl, {
//            method: 'POST',
//            body: formData
//        });

//        if (response.ok) {
//            const result = await response.json();
//            let html = "";
//            // Görseli ekle
//            if (result.filestream) {
//                html += `<img src="data:image/png;base64,${result.filestream}" style="max-width:100%;border:1px solid #ccc;" /><br/>`;
//            }
//            // Nesne listesini ekle
//            if (result.objects && result.objects.length > 0) {
//                html += "<ul>";
//                for (const obj of result.objects) {
//                    html += `<li>${obj.label}: (${obj.x}, ${obj.y}, ${obj.width}, ${obj.height})</li>`;
//                }
//                html += "</ul>";
//            }
//            resultDiv.innerHTML = html;
//        } else {
//            resultDiv.innerHTML = "<span style='color:red'>Bir hata oluştu.</span>";
//        }
//    }
//};

window.imageUploadInterop = {
    uploadImageAndShowResult: async function (inputId, apiUrl, resultDivId, dotNetRef) {
        const input = document.getElementById(inputId);
        const resultDiv = document.getElementById(resultDivId);
        if (!input || !input.files || input.files.length === 0) return;

        const file = input.files[0];
        const formData = new FormData();
        formData.append("file", file, file.name);

        const response = await fetch(apiUrl, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const result = await response.json();
            // Görseli Blazor'a bildir
            if (result.filestream && dotNetRef) {
                dotNetRef.invokeMethodAsync('OnApiResult', `data:image/png;base64,${result.filestream}`);
            }
            // Nesne listesini ekle
            let html = "";
            if (result.objects && result.objects.length > 0) {
                html += "<ul>";
                for (const obj of result.objects) {
                    html += `<li>${obj.label}: (${obj.x}, ${obj.y}, ${obj.width}, ${obj.height})</li>`;
                }
                html += "</ul>";
            }
            resultDiv.innerHTML = html;
        } else {
            resultDiv.innerHTML = "<span style='color:red'>Bir hata oluştu.</span>";
        }
    }
};