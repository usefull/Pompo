var demoService;

const waitForFactory = (timeout = 10) => new Promise((resolve, reject) => {
      let waited = 0
  
      function wait(interval) {
        setTimeout(() => {
          waited += interval
          if (window.dotNetObjectFactory !== undefined) {
            return resolve()
          }
          if (waited >= timeout * 1000) {
            return reject({ message: 'Poco factory initializing timeout' })
          }
          wait(interval)
        }, interval)
      }
  
      wait(100);
    })

export const getDemoService = async () => {
    if (!demoService) {
        await waitForFactory();
        demoService = await window.dotNetObjectFactory.create_demo('mike');
    }
    return demoService;
}

export const getUtilityService = async () => {  
  await waitForFactory();
  return await window.dotNetObjectFactory.resolve_di('WasmModule.UtilityService');
}