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
    Context2InputNumber: () => document.querySelector("#Context2-InputNumber-Id"),
    Context2Checkbox: () => document.querySelector("#Context2-Checkbox-Id"),
    Context2File: () => document.querySelector("#Context2-File-Id"),
    Context2DateTime: () => document.querySelector("#Context2-DateTime-Id"),
    Context2DateTimeLocal: () => document.querySelector("#Context2-DateTimeLocal-Id"),
    Context2SelectContainer: () => document.querySelector("#Context2-Select"),
    Context2Select: () => document.querySelector("#Context2-Select-Id"),
    Context2Option1: () => document.querySelector("#Context2-Option1-Id"),
    Context2Option2: () => document.querySelector("#Context2-Option2-Id"),
    Context2Option3: () => document.querySelector("#Context2-Option3-Id"),
    Context2SelectMultiContainer: () => document.querySelector("#Context2-SelectMulti"),
    Context2SelectMulti: () => document.querySelector("#Context2-SelectMulti-Id"),
    Context2Option4: () => document.querySelector("#Context2-Option4-Id"),
    Context2Option5: () => document.querySelector("#Context2-Option5-Id"),
    Context2Option6: () => document.querySelector("#Context2-Option6-Id"),
    Context2Textarea: () => document.querySelector("#Context2-Textarea-Id"),
    Context2Hyperlink: () => document.querySelector("#Context2-Hyperlink-Id"),
    Context2Image: () => document.querySelector("#Context2-Image-Id"),
    Context2RadioButtons: () => document.querySelector("#Context2-RadioButtons-Id"),
    Context2Radio1: () => document.querySelector("#Context2-Radio1-Id"),
    Context2Radio2: () => document.querySelector("#Context2-Radio2-Id"),
    Context2Radio3: () => document.querySelector("#Context2-Radio3-Id"),
    Context3Div1: () => document.querySelector("#Context3-Div1-Id"),
    Context3Div2: () => document.querySelector("#Context3-Div2-Id"),
    Context3Div3: () => document.querySelector("#Context3-Div3-Id"),
    Context3Span1: () => document.querySelector("#Context3-Span1"),
    Context3Span2: () => document.querySelector("#Context3-Span2"),
    Context3Div3Span1: () => document.querySelector("#Context3-Div3-Span1"),
    Context3Div3Span2: () => document.querySelector("#Context3-Div3-Span2"),
    StaleContext: () => document.querySelector("#StaleContext"),
    StaleContextChild1: () => document.querySelector("#StaleChild1"),
    StaleContextChild2: () => document.querySelector("#StaleChild2"),
    StaleContextChild3: () => document.querySelector("#StaleChild3"),
    StaleContextChild4: () => document.querySelector("#StaleChild4"),
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
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");

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
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");
    element.children[element.childElementCount-1].remove();
}

/**
 *
 * @param {HTMLElement} element
 */
const HideElement = async function(element)
{
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");
    element.style.display = "none";
}

/**
 *
 * @param {HTMLElement} element
 */
const ShowElement = async function(element)
{
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");
    element.style.display = "initial";
}

/**
 *
 * @param {HTMLInputElement} element
 */
const Enable = async function(element)
{
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");
    element.disabled = false;
}

/**
 *
 * @param {HTMLInputElement} element
 */
const Disable = async function(element)
{
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");
    element.disabled = true;
}

/**
 *
 * @param {HTMLElement} element
 */
const Select = async function(element)
{
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");

    if (element.tagName == 'OPTION')
        element.selected = true;
    else if (element.tagName == 'INPUT')
        element.checked = true;

    throw new Error("Not a selectable element!");
}

/**
 *
 * @param {HTMLElement} element
 */
const Deselect = async function(element)
{
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");

    if (element.tagName == 'OPTION')
        element.selected = false;
    else if (element.tagName == 'INPUT')
        element.checked = false;

    throw new Error("Not a selectable element!");
}

/**
 *
 * @param {HTMLElement} element
 * @param {string} name
 * @param {string} value
 */
const ChangeAttribute = async function(element, name, value)
{
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");
    element.setAttribute(name, value);
}

/**
 *
 * @param {HTMLElement} element
 * @param {HTMLElement} text
 */
const ChangeText = async function(element, text)
{
    if (!(element instanceof HTMLElement)) throw new Error("Argument 'element' is not an instance of HTMLElement");
    element.textContent = text;
}

/**
 * 
 * @param {Number} timeout 
 * @param {String} newText 
 */
const KillAndReRenderStaleContext = async function (timeout, newText)
{
    if ( !(newText && (typeof newText === 'string') && newText.length > 0) )
    {
        throw new Error('Argument newText is undefined, not a string or empty');
    }

    let Timeout = timeout && timeout > 0 ? timeout : 0;

    let StaleContextElement = Elements.StaleContext();
    StaleContextElement.innerHTML = '';

    if (Timeout) await Wait(timeout);
    StaleContextElement.innerHTML = `
        <div id="StaleChild1">
            <ul id="StaleChild2">
                <li id="StaleChild3">
                    <span id="StaleChild4">${newText}</span>
                </li>
            </ul>
        </div>`;
}