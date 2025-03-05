import { useEffect, useState } from "react";
import logo from './logo.svg';
import './App.css';
import { getDemoService } from "./DemoService";

function App() {

  const [demoService, setDemoService] = useState(null);

  useEffect(() => {
    getDemoService().then(s => setDemoService(s));
  });

  const onDoSomeWork = (e) => {
    e.preventDefault();
    demoService.do();
  };

  const onSum = async (e) => {
    e.preventDefault();
    var sum = await demoService.sum({ X: 5, Y: 10 });
    console.log(sum);
  };

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a className="App-link" href="#" rel="noopener noreferrer" onClick={onDoSomeWork}>
          Do Some Work
        </a>
        <a className="App-link" href="#" rel="noopener noreferrer" onClick={onSum}>
          Sum
        </a>
      </header>
    </div>
  );
}

export default App;
