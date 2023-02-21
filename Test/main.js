const Elements =
{
    Context1: () => document.querySelector("#Context1"),
    Context2: () => document.querySelector("#Context2"),
    Context3: () => document.querySelector("#Context3"),
    Context1Div1: () => document.querySelector("#Context1-Div1-Id"),
    Context1Div2: () => document.querySelector("#Context1-Div2-Id"),
    Context1Div3: () => document.querySelector("#Context1-Div3-Id"),
    Context2Button: () => document.querySelector("#Context2-Button-Id"),
    Context2InputText: () => document.querySelector("#Context2-InputText-Id"),
    Context2Checkbox: () => document.querySelector("#Context2-Checkbox-Id"),
    Context2File: () => document.querySelector("#Context2-File-Id"),
    Context2DateTime: () => document.querySelector("#Context2-DateTime-Id"),
    Context2DateTimeLocal: () => document.querySelector("#Context2-DateTimeLocal-Id"),
    Context2Dropdown: () => document.querySelector("#Context2-Dropdown-Id"),
    Context2Select: () => document.querySelector("#Context2-Select-Id"),
    Context2Option1: () => document.querySelector("#Context2-Option1-Id"),
    Context2Option2: () => document.querySelector("#Context2-Option2-Id"),
    Context2Option3: () => document.querySelector("#Context2-Option3-Id"),
    Context2Textarea: () => document.querySelector("#Context2-Textarea-Id"),
    Context2RadioButtons: () => document.querySelector("#Context2-RadioButtons-Id"),
    Context2Radio1: () => document.querySelector("#Context2-Radio1-Id"),
    Context2Radio2: () => document.querySelector("#Context2-Radio2-Id"),
    Context2Radio3: () => document.querySelector("#Context2-Radio3-Id"),
    Context3Div1: () => document.querySelector("#Context3-Div1-Id"),
    Context3Div2: () => document.querySelector("#Context3-Div2-Id"),
    Context3Div3: () => document.querySelector("#Context3-Div3-Id")
}

window.onload = function()
{
    Elements.Context2Button().addEventListener("click", ()=> window.alert("I have been clicked!"));
}

/**
 *
 * @param {Number} timeout
 * @returns A promse that resolves once the timeout is complete
 */
const Wait = async function (timeout)
{
    return new Promise(resolve=> setTimeout(resolve, timeout));
}

/**
 *
 * @param {HTMLElement} element
 */
const CopyLastChildOfParentAndAppend = async function(element)
{
    let LastChild = element.children[element.childElementCount-1];
    let NewChild = LastChild.cloneNode(true);
    NewChild.id = NewChild.id + "0";
    element.appendChild(NewChild);
}

/**
 *
 * @param {HTMLElement} element
 */
const RemoveLastChildOfParent = async function(element)
{
    element.children[element.childElementCount-1].remove();
}

/**
 *
 * @param {HTMLElement} element
 */
const HideElement = async function(element)
{
    element.style.display = "none";
}

/**
 *
 * @param {HTMLElement} element
 */
const ShowElement = async function(element)
{
    element.style.display = "initial";
}

/**
 *
 * @param {HTMLInputElement} element
 */
const Enable = async function(element)
{
    element.disabled = false;
}

/**
 *
 * @param {HTMLInputElement} element
 */
const Disable = async function(element)
{
    element.disabled = true;
}

/**
 *
 * @param {HTMLElement} element
 */
const Select = async function(element)
{
    if (element.tagName == 'OPTION')
        element.selected = true;
    else if (element.tagName == 'INPUT')
        element.checked = true;
}

/**
 *
 * @param {HTMLElement} element
 */
const Deselect = async function(element)
{
    if (element.tagName == 'OPTION')
        element.selected = false;
    else if (element.tagName == 'INPUT')
        element.checked = false;
}

/**
 *
 * @param {HTMLElement} element
 * @param {Number} waitTime
 */
const ChangeAttribute = async function(element, name, value)
{
    element.setAttribute(name, value);
}

/**
 *
 * @param {HTMLElement} element
 */
const ChangeText = async function(element, text)
{
    element.textContent = text;
}
