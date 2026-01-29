/*************************************************
 * 1️⃣ REGISTER REQUIRED BLOTS
 *************************************************/

const ImageBlot = Quill.import('formats/image');
Quill.register(ImageBlot, true);

const BlockEmbed = Quill.import('blots/block/embed');

class DividerBlot extends BlockEmbed {
    static create() {
        const node = super.create();
        node.innerHTML = '<hr />';
        return node;
    }
}
DividerBlot.blotName = 'divider';
DividerBlot.tagName = 'div';
Quill.register(DividerBlot, true);


/*************************************************
 * 2️⃣ INIT QUILL
 *************************************************/

const quill = new Quill('#editor', {
    theme: 'snow',
    placeholder: 'Tell your story…',
    modules: {
        toolbar: false
    }
});


/*************************************************
 * 3️⃣ ELEMENT REFERENCES
 *************************************************/

const floatingPlus = document.getElementById('floatingPlus');
const floatingToolbar = document.getElementById('floatingToolbar');
const imageInput = document.getElementById('imageInput');
const writeForm = document.getElementById('writeForm');

let savedRange = null;


/*************************************************
 * 4️⃣ SELECTION HANDLING (SINGLE SOURCE OF TRUTH)
 *************************************************/

quill.on('selection-change', function (range) {

    if (!range) {
        hideToolbar();
        hidePlus();
        return;
    }

    savedRange = range;

    /* ===============================
       TEXT SELECTED → TOOLBAR
    ================================*/
    if (range.length > 0) {
        const bounds = quill.getBounds(range.index, range.length);

        floatingToolbar.style.left =
            bounds.left + bounds.width / 2 + 'px';
        floatingToolbar.style.top =
            bounds.top - 8 + 'px';

        floatingToolbar.style.display = 'flex';
        floatingToolbar.classList.add('show');

        hidePlus();
        updateActiveStates();
        return;
    }

    /* ===============================
       CARET ONLY → PLUS BUTTON
    ================================*/
    hideToolbar();

    const [line] = quill.getLine(range.index);
    if (!line) {
        hidePlus();
        return;
    }

    const text = line.domNode.innerText.trim();

    if (text === '') {
        const bounds = quill.getBounds(range.index);
        floatingPlus.style.top = bounds.top + 'px';
        floatingPlus.style.display = 'flex';
    } else {
        hidePlus();
    }
});


/*************************************************
 * 5️⃣ TOOLBAR ACTIONS
 *************************************************/

floatingToolbar.addEventListener('click', function (e) {
    e.preventDefault();

    const button = e.target.closest('button');
    if (!button) return;

    const format = button.dataset.format;

    if (format === 'link') {
        const url = prompt('Enter link URL');
        if (url) quill.format('link', url);
    } else {
        const isActive = quill.getFormat()[format];
        quill.format(format, !isActive);
    }

    updateActiveStates();
});

function updateActiveStates() {
    const formats = quill.getFormat();

    floatingToolbar.querySelectorAll('button').forEach(btn => {
        const format = btn.dataset.format;
        btn.classList.toggle('active', !!formats[format]);
    });
}


/*************************************************
 * 6️⃣ DIVIDER INSERT
 *************************************************/

document.getElementById('addDivider').addEventListener('click', () => {
    quill.focus();

    const range = savedRange || { index: quill.getLength(), length: 0 };

    quill.insertEmbed(range.index, 'divider', true, 'user');
    quill.insertText(range.index + 1, '\n');
    quill.setSelection(range.index + 2, 0);
});


/*************************************************
 * 7️⃣ IMAGE INSERT (AJAX UPLOAD → URL)
 *************************************************/

document.getElementById('addImage').addEventListener('click', () => {
    imageInput.click();
});

imageInput.addEventListener('change', function () {
    const file = this.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('file', file);

    const range = savedRange || { index: quill.getLength(), length: 0 };

    fetch('/Post/UploadImage', {
        method: 'POST',
        body: formData
    })
        .then(res => {
            if (!res.ok) throw new Error('Upload failed');
            return res.json();
        })
        .then(data => {
            quill.insertEmbed(range.index, 'image', data.url);
            quill.insertText(range.index + 1, '\n');
            quill.setSelection(range.index + 2, 0);
        })
        .catch(err => {
            console.error(err);
            alert('Image upload failed');
        });

    this.value = '';
});


/*************************************************
 * 8️⃣ FORM SUBMIT → SAVE HTML
 *************************************************/

writeForm.addEventListener('submit', () => {
    document.getElementById('content').value = quill.root.innerHTML;
});


/*************************************************
 * 9️⃣ HELPERS
 *************************************************/

function hideToolbar() {
    floatingToolbar.classList.remove('show');
    floatingToolbar.style.display = 'none';
}

function hidePlus() {
    floatingPlus.style.display = 'none';
}


/*************************************************
 * 🔟 HEADER DROPDOWN (CLICK ONLY)
 *************************************************/

const dropdown = document.querySelector('.header-dropdown');
const dropdownBtn = dropdown.querySelector('.dropdown-btn');

dropdownBtn.addEventListener('click', function (e) {
    e.stopPropagation();
    dropdown.classList.toggle('open');
});

document.addEventListener('click', function () {
    dropdown.classList.remove('open');
});
