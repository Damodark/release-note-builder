window.BlazorDownloadFile = function (data) {
    const fileName = data.fileName;
    const byteArray = new Uint8Array(data.byteArray);
    const blob = new Blob([byteArray], { type: data.mimeType });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
};