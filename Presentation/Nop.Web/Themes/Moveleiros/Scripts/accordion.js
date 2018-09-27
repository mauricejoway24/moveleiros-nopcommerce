class AccordionItem {

    constructor() {
        this._el = null
        this._disable = false
    }

    disableAccordion() {
        this._disable = true
    }

    setElement(element) {
        this._el = element
        this._el.addEventListener('click', () => this.onClick())
    }

    onClick() {
        if (this._disable) return

        let parent = this._el.parentElement
        let elClass = parent.classList

        if (elClass.contains('open'))
            elClass.remove('open')
        else
            elClass.add('open')
    }
}

class Accordion {

    constructor() {
        this._elements = []

        this.bindEvents()
    }

    bindEvents() {
        document
            .querySelectorAll('[data-accordion] [data-control]')
            .forEach(accordion => {
                let el = new AccordionItem()
                el.setElement(accordion)
                this._elements.push(el)
            })
    }

    reload() {
        this._elements.forEach(accordion => { accordion.disableAccordion() })
        this._elements = []
        this.bindEvents()
    }
}

window.addEventListener('load', _ => {
    window.AccordionControl = new Accordion()
})