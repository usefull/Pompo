import logo from './logo.svg';
import './App.css';
import { getDemoService, getUtilityService } from "./DemoService";

function App() {

  const onDoSomeWork = async (e) => {
    e.preventDefault();

    let demo = await getDemoService();
    demo.do();
  };

  const onSum = async (e) => {
    e.preventDefault();

    let demo = await getDemoService();
    var sum = await demo.sum({ x: 5, y: 10 });
    console.log(sum);
  };

  const onDI = async (e) => {
    e.preventDefault();

    let us = await getUtilityService();
    //console.log(sum);
  };

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a className="App-link" href="#" rel="noopener noreferrer" onClick={onDoSomeWork}>Do Some Work</a>
        <a className="App-link" href="#" rel="noopener noreferrer" onClick={onSum}>Sum</a>
        <a className="App-link" href="#" rel="noopener noreferrer" onClick={onDI}>DI</a>
      </header>
    </div>
  );
}

export default App;
