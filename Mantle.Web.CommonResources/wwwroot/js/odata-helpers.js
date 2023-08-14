class ODataHelper {
    static #defaultOptions = {
        messages: {
            getRecordError: "Error when trying to retrieve record!",
            deleteRecordConfirm: "Are you sure that you want to delete this record?",
            deleteRecordSuccess: "Successfully deleted record!",
            deleteRecordError: "Error when trying to retrieve record!",
            insertRecordSuccess: "Successfully inserted record!",
            insertRecordError: "Error when trying to insert record!",
            updateRecordSuccess: "Successfully updated record!",
            updateRecordError: "Error when trying to update record!"
        }
    };

    static options = this.#defaultOptions;

    static async getOData(url, onError) {
        return await fetch(url)
            .then(response => response.json())
            .catch(error => {
                if (onError) {
                    onError();
                }
                else {
                    //$.notify({ message: this.options.messages.getRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                    MantleNotify.error(this.options.messages.getRecordError);
                }
                console.error('Error: ', error);
            });
    }

    static async deleteOData(url, onSuccess, onError) {
        if (confirm(this.options.messages.deleteRecordConfirm)) {
            await fetch(url, { method: 'DELETE' })
                .then(response => {
                    if (response.ok) {
                        if (onSuccess) {
                            onSuccess();
                        }
                        else {
                            this.refreshODataGrid();
                            //$.notify({ message: this.options.messages.deleteRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
                            MantleNotify.success(this.options.messages.deleteRecordSuccess);
                        }
                    } else {
                        if (onError) {
                            onError();
                        }
                        else {
                            //$.notify({ message: this.options.messages.deleteRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                            MantleNotify.error(this.options.messages.deleteRecordError);
                        }
                    }
                })
                .catch(error => {
                    if (onError) {
                        onError();
                    }
                    else {
                        //$.notify({ message: this.options.messages.deleteRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                        MantleNotify.error(this.options.messages.deleteRecordError);
                    }
                    console.error('Error: ', error);
                });
        }
    }

    static async postOData(url, record, onSuccess, onError) {
        return await fetch(url, {
            method: "POST",
            headers: {
                'Content-type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(record)
        })
        .then(response => {
            if (response.ok) {
                if (onSuccess) {
                    onSuccess();
                }
                else {
                    this.refreshODataGrid();
                    switchSection($("#grid-section"));
                    //$.notify({ message: this.options.messages.insertRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
                    MantleNotify.success(this.options.messages.insertRecordSuccess);
                }
            }
            else {
                if (onError) {
                    onError();
                }
                else {
                    //$.notify({ message: this.options.messages.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                    MantleNotify.error(this.options.messages.insertRecordError);
                }
            }
            return response;
        })
        .catch(error => {
            if (onError) {
                onError();
            }
            else {
                //$.notify({ message: this.options.messages.insertRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                MantleNotify.error(this.options.messages.insertRecordError);
            }
            console.error('Error: ', error);
        });
    }

    static async putOData(url, record, onSuccess, onError) {
        return await fetch(url, {
            method: "PUT",
            headers: {
                'Content-type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(record)
        })
        .then(response => {
            if (response.ok) {
                if (onSuccess) {
                    onSuccess();
                }
                else {
                    this.refreshODataGrid();
                    switchSection($("#grid-section"));
                    //$.notify({ message: this.options.messages.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
                    MantleNotify.success(this.options.messages.updateRecordSuccess);
                }
            }
            else {
                if (onError) {
                    onError();
                }
                else {
                    //$.notify({ message: this.options.messages.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                    MantleNotify.error(this.options.messages.updateRecordError);
                }
            }
            return response;
        })
        .catch(error => {
            if (onError) {
                onError();
            }
            else {
                //$.notify({ message: this.options.messages.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                MantleNotify.error(this.options.messages.updateRecordError);
            }
            console.error('Error: ', error);
        });
    }

    static async patchOData(url, patch, onSuccess, onError) {
        return await fetch(url, {
            method: "PATCH",
            headers: {
                'Content-type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(patch)
        })
        .then(response => {
            if (response.ok) {
                if (onSuccess) {
                    onSuccess();
                }
                else {
                    this.refreshODataGrid();
                    //$.notify({ message: this.options.messages.updateRecordSuccess, icon: 'fa fa-check' }, { type: 'success' });
                    MantleNotify.success(this.options.messages.updateRecordSuccess);
                }
            }
            else {
                if (onError) {
                    onError();
                }
                else {
                    //$.notify({ message: this.options.messages.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                    MantleNotify.error(this.options.messages.updateRecordError);
                }
            }
            return response;
        })
        .catch(error => {
            if (onError) {
                onError();
            }
            else {
                //$.notify({ message: this.options.messages.updateRecordError, icon: 'fa fa-exclamation-triangle' }, { type: 'danger' });
                MantleNotify.error(this.options.messages.updateRecordError);
            }
            console.error('Error: ', error);
        });
    }

    static async refreshODataGrid() {
        const grid = $('#Grid').data('kendoGrid');
        if (grid) {
            grid.dataSource.read();
            grid.refresh();
        }
    };
}