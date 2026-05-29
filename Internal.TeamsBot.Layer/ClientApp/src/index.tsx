import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";
import reportWebVitals from "./reportWebVitals";
import 'bootstrap/dist/css/bootstrap.min.css';
import AlertDialogProvider from "./components/common/AlertDialogProvider";

ReactDOM.render(
  <React.StrictMode>
    <AlertDialogProvider>
      <App />
    </AlertDialogProvider>
  </React.StrictMode>,
  document.getElementById("root")
);

reportWebVitals();
