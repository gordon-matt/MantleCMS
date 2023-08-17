const emptyGuid = '00000000-0000-0000-0000-000000000000';

function getLocalStorageKeys() {
    const keys = [];
    for (const i = 0; i < localStorage.length; i++) {
        keys[i] = localStorage.key(i);
    }
    return keys;
};