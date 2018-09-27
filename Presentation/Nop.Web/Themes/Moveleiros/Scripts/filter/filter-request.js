class Soixa {

    get(url, callback, headers) {
        let req = new XMLHttpRequest()

        req.onreadystatechange = () => {
            if (req.readyState == XMLHttpRequest.DONE) {
                if (callback) callback(null, req.response)
            }
        }

        req.onerror = (err) => {
            if (callback) callback(err, null)
        }

        req.open('GET', url, true)

        for (key in headers) {
            req.setRequestHeader(key, headers[key])
        }

        req.setRequestHeader('X-Requested-With', 'XMLHttpRequest')

        req.send(null)
    }

    post(url, data, callback, headers) {
        let req = new XMLHttpRequest()

        req.onreadystatechange = () => {
            if (req.readyState == XMLHttpRequest.DONE) {
                if (callback) callback(null, req.response)
            }
        }

        req.onerror = (err) => {
            if (callback) callback(err, null)
        }

        for (key in headers) {
            req.setRequestHeader(key, headers[key])
        }

        req.open('POST', url, true)
        req.send(data)
    }
}

class FilterRequest {
    constructor() {
        this._axios = new Soixa()

        this.bindEvent()
    }

    bindEvent() {
        window.getProducts = (c, s, f, d) => this.getProducts(c, s, f, d)
    }

    getProducts(curl, successCallback, failCallback, doneCallback) {
        let queryString = curl.length > 1 ? curl + '&ajax=true' : curl;

        this._axios.get(`/catalog/search${queryString}`, (err, data) => {
            if (err) {
                if (failCallback) failCallback(err)
                return
            }

            if (successCallback) successCallback(data)
            if (doneCallback) doneCallback()
        })
    }
}

new FilterRequest()