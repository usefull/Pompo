//////////////////////////////////////////
// The factory for transmit .NET objects.
// Generated by Pompo 06.03.2025 10:49:32.
//////////////////////////////////////////

window.transmitFunc = (obj) => {
    window.dotNetObjectFactory = obj;
    
    window.dotNetObjectFactory.create_demo = async (id) => {
        let o = null;
        window.transmitFunc = (obj) => o = obj;
        await window.dotNetObjectFactory.invokeMethodAsync(
            'Create_demo',
            id
        );

        o.do = async () => await o.invokeMethodAsync(
            'do'
        );

        o.sum = async (request) => await o.invokeMethodAsync(
            'sum',
            request
        );

        delete window.transmitFunc;
        return o;
    };
    console.log('Pompo factory initialized.');
};

export function transmit(obj) {
    if (window.transmitFunc) {
        window.transmitFunc(obj);
    }
};
