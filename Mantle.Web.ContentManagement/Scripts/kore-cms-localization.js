async function Translate(key) {
    await fetch(`/admin/localization/localizable-strings/translate/${encodeURIComponent(key) }`)
        .then(response => response.json())
        .then((data) => {
            return data.Translation;
        })
        .catch(error => {
            console.error('Error: ', error);
        });
}