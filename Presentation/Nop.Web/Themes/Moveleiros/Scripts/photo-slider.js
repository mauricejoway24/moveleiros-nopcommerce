class PhotoSlider {
    constructor(elSel) {
        this.element = null

        this.selectElement(elSel)
        this.bindElementEvents()
    }

    selectElement(elSel) {
        this.element = document.querySelector(elSel)
    }

    bindElementEvents() {
        console.log('element', this.element)
    }
}
