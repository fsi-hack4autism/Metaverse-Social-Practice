import * as React from "react";
import { useEffect } from "react";
import "./App.css";
import logo from "./verscraft_logo.svg";
import cafe from "./cafe.png";
import resto from "./restaurant.png";

export default function App() {
  const [checkedOne, setCheckedOne] = React.useState(false);
  const [checkedTwo, setCheckedTwo] = React.useState(false);
  const WELCOME_PHRASE = "Welcome! Let's set up your session.";
  const handleChangeOne = () => {
    setCheckedOne(!checkedOne);
  };

  const handleChangeTwo = () => {
    setCheckedTwo(!checkedTwo);
  };

  const printToConsole = () => {
    console.log("checkedOne: ", checkedOne);
    console.log("checkedTwo: ", checkedTwo);
  };
  useEffect(() => {
    printToConsole();
  }, [checkedOne, checkedTwo]);

  function handleSubmit(e) {
    // Prevent the browser from reloading the page
    e.preventDefault();

    // Read the form data
    const form = e.target;
    const formData = new FormData(form);

    // You can pass formData as a fetch body directly:
    //fetch("/some-api", { method: form.method, body: formData });

    // Or you can work with it as a plain object:
    const formJson = Object.fromEntries(formData.entries());
    console.log(formJson);
  }

  return (
    <div className="App">
      <img src={logo} alt="Verse Craft logo with speech bubble icon"></img>
      <header className="App-header">
        <div class="title">
          <p>{WELCOME_PHRASE}</p>
        </div>
        <form className="set-up-form" method="post" onSubmit={handleSubmit}>
          <div class="row-1">
            <div class="column">
              <div class="blue-column">
                <label>Your Name:</label>
                <input type="text" name="name" id="name" />
              </div>
            </div>
            <div class="column">
              <div class="green-column">
                <label>Select a template</label>
                <br></br>
                <img
                  src={cafe}
                  alt="A luminous cafe with windows, plenty of seating and a counter"
                  class="template-image"
                ></img>
                <img
                  src={resto}
                  alt="A moody chandelier-lit restaurant"
                  class="template-image"
                ></img>
              </div>
            </div>
          </div>
          <div class="row-2">
            <div class="column">
              <div class="blue-column">
                <label>Your goals for this session:</label>
                <div class="goals-checkboxes">
                  <Checkbox
                    label="Goal 1"
                    value={checkedOne}
                    onChange={handleChangeOne}
                  />
                  <br></br>
                  <Checkbox
                    label="Goal 2"
                    value={checkedTwo}
                    onChange={handleChangeTwo}
                  />
                </div>
              </div>
            </div>
            <div class="column">
              <div class="green-column">
                <label>Preference setting</label>
                <br></br>
                <label>Cashier personality</label>
                <select>
                  <option value="quick">Speaking quickly</option>
                  <option value="mumble">Mumbling</option>
                  <option value="friendly">Friendly</option>
                  <option value="grumpy">Grumpy</option>
                  <option value="quiet">Quiet</option>
                  <option value="loud">Loud</option>
                </select>
              </div>
            </div>
          </div>
          <input type="submit" value="Submit" class="submit-button" />
        </form>
      </header>
    </div>
  );
}
const Checkbox = ({ label, value, onChange }) => {
  return (
    <label>
      <input type="checkbox" checked={value} onChange={onChange} />
      {label}
    </label>
  );
};
