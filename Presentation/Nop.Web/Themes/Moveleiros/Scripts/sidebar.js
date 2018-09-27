class Sidebar {
    constructor(trigger, popupElement, direction) {
        this.trigger = trigger
        this.popupElement = popupElement
        this.direction = direction

        this._init()
    }

    _init() {
        let btnElement = document.querySelector(this.trigger)
        let pop = document.querySelector(this.popupElement)

        if (!btnElement || !pop) return

        pop.addEventListener('click', () => { this.close() })
        btnElement.addEventListener('click', () => { this.show(this) })
    }

    show(ctx) {
        let c = ctx || this
        let popupElement = document.querySelector(c.popupElement)

        document.body.classList.add('open')

        if (this.direction === 'right')
            document.body.classList.add('open-right')
        else
            document.body.classList.add('open-left')
    }

    close() {
        document.body.classList.remove('open')
        document.body.classList.remove('open-right')
        document.body.classList.remove('open-left')
    }
}