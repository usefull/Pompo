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

    /////////////////////////////////////////////////////////////
    window.dotNetObjectFactory.resolve_di = async (type) => {
        let o = null;
        window.transmitFunc = (obj, script) => {
            console.log(script);
            let fn = Function("obj", script);
            o = fn(obj);
        };
        await window.dotNetObjectFactory.invokeMethodAsync(
            'ResolveDI',
            type
        );
        delete window.transmitFunc;
        return o;
    };

    delete window.transmitFunc;
    //////////////////////////////////////////////////////////////
    console.log('Pompo factory initialized.');
};

export function transmit(obj, script) {
    if (window.transmitFunc) {
        window.transmitFunc(obj, script);
    }
};