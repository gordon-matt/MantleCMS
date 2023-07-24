async function getOData(url) {
    return await fetch(url)
        .then(response => response.json())
        .catch(error => {
            //$.notify({ message: "Error when trying to retrieve record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            $.notify("Error when trying to retrieve record!", 'danger');
            console.error('Error: ', error);
        });
}

async function deleteOData(url) {
    if (confirm("Are you sure that you want to delete this record?")) {
        await fetch(url, { method: 'DELETE' })
            .then(response => {
                if (response.ok) {
                    refreshODataGrid();
                    //$.notify({ message: "Successfully deleted record!", icon: 'fa fa-check' }, { type: 'success' });
                    $.notify("Successfully deleted record!", 'success');
                } else {
                    //$.notify({ message: "Error when trying to retrieve record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                    $.notify("Error when trying to retrieve record!", 'danger');
                }
            })
            .catch(error => {
                //$.notify({ message: "Error when trying to retrieve record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                $.notify("Error when trying to retrieve record!", 'danger');
                console.error('Error: ', error);
            });
    }
}

async function postOData(url, record) {
    return await fetch(url, {
        method: "POST",
        headers: {
            'Content-type': 'application/json; charset=utf-8'
        },
        body: JSON.stringify(record)
    })
    .then(response => {
        if (response.ok) {
            refreshODataGrid();
            switchSection($("#grid-section"));
            //$.notify({ message: "Successfully inserted record!", icon: 'fa fa-check' }, { type: 'success' });
            $.notify("Successfully inserted record!", 'success' );
        }
        else {
            //$.notify({ message: "Error when trying to insert record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            $.notify("Error when trying to insert record!", 'danger' );
        }
        return response;
    })
    .catch(error => {
        //$.notify({ message: "Error when trying to insert record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        $.notify("Error when trying to insert record!", 'danger' );
        console.error('Error: ', error);
    });
}

async function putOData(url, record) {
    return await fetch(url, {
        method: "PUT",
        headers: {
            'Content-type': 'application/json; charset=utf-8'
        },
        body: JSON.stringify(record)
    })
    .then(response => {
        if (response.ok) {
            refreshODataGrid();
            switchSection($("#grid-section"));
            //$.notify({ message: "Successfully updated record!", icon: 'fa fa-check' }, { type: 'success' });
            $.notify("Successfully updated record!", 'success' );
        }
        else {
            //$.notify({ message: "Error when trying to update record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            $.notify("Error when trying to update record!", 'danger' );
        }
        return response;
    })
    .catch(error => {
        //$.notify({ message: "Error when trying to update record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        $.notify("Error when trying to update record!", 'danger' );
        console.error('Error: ', error);
    });
}

async function patchOData(url, patch) {
    return await fetch(url, {
        method: "PATCH",
        headers: {
            'Content-type': 'application/json; charset=utf-8'
        },
        body: JSON.stringify(patch)
    })
    .then(response => {
        if (response.ok) {
            refreshODataGrid();
            //$.notify({ message: "Successfully updated record!", icon: 'fa fa-check' }, { type: 'success' });
            $.notify("Successfully updated record!", 'success' );
        }
        else {
            //$.notify({ message: "Error when trying to update record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
            $.notify("Error when trying to update record!", 'danger' );
        }
        return response;
    })
    .catch(error => {
        //$.notify({ message: "Error when trying to update record!", icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
        $.notify("Error when trying to update record!", 'danger' );
        console.error('Error: ', error);
    });
}

function refreshODataGrid() {
    $('#Grid').data('kendoGrid').dataSource.read();
    $('#Grid').data('kendoGrid').refresh();
};