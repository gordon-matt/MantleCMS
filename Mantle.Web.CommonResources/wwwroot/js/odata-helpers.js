﻿class ODataHelper {
    static #defaultOptions = {
        messages: {
            getRecordError: MantleI18N.t('Mantle.Web/General.GetRecordError'),
            deleteRecordConfirm: MantleI18N.t('Mantle.Web/General.ConfirmDeleteRecord'),
            deleteRecordSuccess: MantleI18N.t('Mantle.Web/General.DeleteRecordSuccess'),
            deleteRecordError: MantleI18N.t('Mantle.Web/General.DeleteRecordError'),
            insertRecordSuccess: MantleI18N.t('Mantle.Web/General.InsertRecordSuccess'),
            insertRecordError: MantleI18N.t('Mantle.Web/General.InsertRecordError'),
            updateRecordSuccess: MantleI18N.t('Mantle.Web/General.UpdateRecordSuccess'),
            updateRecordError: MantleI18N.t('Mantle.Web/General.UpdateRecordError')
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
                            GridHelper.refreshGrid();
                            MantleNotify.success(this.options.messages.deleteRecordSuccess);
                        }
                    } else {
                        if (onError) {
                            onError();
                        }
                        else {
                            MantleNotify.error(this.options.messages.deleteRecordError);
                        }
                    }
                })
                .catch(error => {
                    if (onError) {
                        onError();
                    }
                    else {
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
                    GridHelper.refreshGrid();
                    switchSection($("#grid-section"));
                    MantleNotify.success(this.options.messages.insertRecordSuccess);
                }
            }
            else {
                if (onError) {
                    onError();
                }
                else {
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
                    GridHelper.refreshGrid();
                    switchSection($("#grid-section"));
                    MantleNotify.success(this.options.messages.updateRecordSuccess);
                }
            }
            else {
                if (onError) {
                    onError();
                }
                else {
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
                    GridHelper.refreshGrid();
                    MantleNotify.success(this.options.messages.updateRecordSuccess);
                }
            }
            else {
                if (onError) {
                    onError();
                }
                else {
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
                MantleNotify.error(this.options.messages.updateRecordError);
            }
            console.error('Error: ', error);
        });
    }
}