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
    modules: { toolbar: false }
});

const contentInput = document.getElementById('content');
if (contentInput && contentInput.value) {
    quill.root.innerHTML = contentInput.value;
}


/*************************************************
 * 3️⃣ GLOBAL STATE (DRAFT)
 *************************************************/
let autoSaveTimer = null;
let draftPostId = null;
let lastSavedContent = '';

const titleInput = document.querySelector('.post-title');
const draftPostIdInput = document.getElementById('DraftPostId');

// Draft edit sync
if (draftPostIdInput && draftPostIdInput.value) {
    draftPostId = draftPostIdInput.value;
}


/*************************************************
 * 4️⃣ AUTO SAVE ENABLE CHECK
 *************************************************/
const autoSaveEnabled = !window.isPublishedPost;

function triggerAutoSave() {
    clearTimeout(autoSaveTimer);
    autoSaveTimer = setTimeout(autoSaveDraft, 1000);
}

if (autoSaveEnabled) {
    quill.on('text-change', triggerAutoSave);

    if (titleInput) {
        titleInput.addEventListener('input', triggerAutoSave);
    }
}


/*************************************************
 * 5️⃣ AUTO SAVE FUNCTION
 *************************************************/
function autoSaveDraft() {

    const content = quill.root.innerHTML;
    const title = titleInput ? titleInput.value.trim() : '';

    // Ignore empty editor + empty title
    if (!title && (!content || content === '<p><br></p>')) return;

    // Ignore unchanged content
    if (content === lastSavedContent) return;

    fetch('/Blogger/Post/AutoSaveDraft', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            Id: draftPostId,
            title: title,
            content: content
        })
    })
        .then(res => res.json())
        .then(data => {
            draftPostId = data.postId;
            lastSavedContent = content;

            // sync hidden input
            if (draftPostIdInput) {
                draftPostIdInput.value = data.postId;
            }

            showSavedStatus(`Draft saved at ${data.savedAt}`);
        })
        .catch(() => {
            showSavedStatus('Auto-save failed');
        });
}


/*************************************************
 * 6️⃣ ELEMENT REFERENCES
 *************************************************/
const floatingPlus = document.getElementById('floatingPlus');
const floatingToolbar = document.getElementById('floatingToolbar');
const imageInput = document.getElementById('imageInput');
const writeForm = document.getElementById('writeForm');

let savedRange = null;


/*************************************************
 * 7️⃣ TITLE AUTO HEIGHT
 *************************************************/
if (titleInput) {
    titleInput.style.height = 'auto';
    titleInput.style.height = titleInput.scrollHeight + 'px';

    titleInput.addEventListener('input', () => {
        titleInput.style.height = 'auto';
        titleInput.style.height = titleInput.scrollHeight + 'px';
    });
}


/*************************************************
 * 8️⃣ SELECTION HANDLING
 *************************************************/
quill.on('selection-change', function (range) {

    if (!range) {
        hideToolbar();
        hidePlus();
        return;
    }

    savedRange = range;

    // Text selected → toolbar
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

    hideToolbar();

    // Caret only → plus
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
 * 9️⃣ TOOLBAR ACTIONS
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
 * 🔟 DIVIDER INSERT
 *************************************************/
document.getElementById('addDivider')?.addEventListener('click', () => {
    quill.focus();

    const range = savedRange || { index: quill.getLength(), length: 0 };

    quill.insertEmbed(range.index, 'divider', true);
    quill.insertText(range.index + 1, '\n');
    quill.setSelection(range.index + 2, 0);
});


/*************************************************
 * 🔟 IMAGE INSERT
 *************************************************/
document.getElementById('addImage')?.addEventListener('click', () => {
    imageInput.click();
});

imageInput?.addEventListener('change', function () {
    const file = this.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('file', file);

    const range = savedRange || { index: quill.getLength(), length: 0 };

    fetch('/Blogger/Post/UploadImage', {
        method: 'POST',
        body: formData
    })
        .then(res => res.json())
        .then(data => {
            quill.insertEmbed(range.index, 'image', data.url);
            quill.insertText(range.index + 1, '\n');
            quill.setSelection(range.index + 2, 0);
        });

    this.value = '';
});


/*************************************************
 * 🔟 FORM SUBMIT (PUBLISH)
 *************************************************/
writeForm?.addEventListener('submit', () => {
    contentInput.value = quill.root.innerHTML;
});


/*************************************************
 * 🔹 HELPERS
 *************************************************/
function hideToolbar() {
    floatingToolbar.classList.remove('show');
    floatingToolbar.style.display = 'none';
}

function hidePlus() {
    floatingPlus.style.display = 'none';
}

function showSavedStatus(text) {
    const el = document.getElementById('autosaveStatus');
    if (!el) return;
    el.innerText = text;
}


/*************************************************
 * 🔟 HEADER DROPDOWN
 *************************************************/
const dropdown = document.querySelector('.header-dropdown');
const dropdownBtn = dropdown?.querySelector('.dropdown-btn');

dropdownBtn?.addEventListener('click', function (e) {
    e.stopPropagation();
    dropdown.classList.toggle('open');
});

document.addEventListener('click', function () {
    dropdown?.classList.remove('open');
});


/*************************************************
 * 🔟 PUBLISH MODAL
 *************************************************/
const openPublishModalBtn = document.getElementById('openPublishModal');
const publishModalEl = document.getElementById('publishModal');

if (openPublishModalBtn && publishModalEl) {
    const publishModal = new bootstrap.Modal(publishModalEl);
    openPublishModalBtn.addEventListener('click', () => {
        publishModal.show();
    });
}
