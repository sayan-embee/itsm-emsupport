import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";
import reportWebVitals from "./reportWebVitals";
import 'bootstrap/dist/css/bootstrap.min.css';
import AlertDialogProvider from './store/AlertDialogProvider';
import { DataProvider } from "./store/DataProvider";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import ErrorBoundaryComponent from "./components/common/ErrorBoundaryComponent";

if (process.env.NODE_ENV !== 'development') {
  console.log = () => { };
  console.debug = () => { };
  console.info = () => { };
  console.warn = () => { };
  // console.error = () => {};
}

try {
  if (process.env.NODE_ENV === 'development') {
    console.groupCollapsed('🖥️ Device Info Summary');

    console.log('Viewport Size:', `${window.innerWidth} x ${window.innerHeight}`);
    console.log('Screen Size:', `${window.screen.width} x ${window.screen.height}`);
    console.log('Device Pixel Ratio:', window.devicePixelRatio);

    const userAgent = navigator.userAgent;

    let agent = "Unknown";
    if (/Mobile|Android|iPhone|iPad/i.test(userAgent)) agent = "Mobile";
    else if (/Mac|Windows|Linux/i.test(userAgent)) agent = "Desktop";
    console.log('User Agent:', agent);

    let application = "Unknown";
    if (userAgent.includes("Edg/")) application = "Microsoft Edge";
    else if (userAgent.includes("Chrome/") && userAgent.includes("Safari/")) application = "Google Chrome";
    else if (userAgent.includes("Safari/") && !userAgent.includes("Chrome/")) application = "Apple Safari";
    else if (userAgent.includes("Firefox/")) application = "Mozilla Firefox";
    else if (userAgent.includes("MSIE") || userAgent.includes("Trident/")) application = "Internet Explorer";
    console.log('Application:', application);

    console.log('Platform:', navigator.platform);
    console.log('Language:', navigator.language);

    const zoomLevel = Math.round((window.innerWidth / document.documentElement.clientWidth) * 100);
    console.log('Estimated Zoom Level:', `${zoomLevel}%`);

    console.log('Timezone:', Intl.DateTimeFormat().resolvedOptions().timeZone);

    console.groupEnd();
  }
}
catch (err) {

}


window.onerror = (message, source, lineno, colno, error) => {
  console.log("Global error caught:", message, source, lineno, colno, error);
  return true; // Prevents the default error overlay
};

window.onunhandledrejection = (event) => {
  console.log("Unhandled promise rejection:", event.reason);
  return true; // Prevents the error overlay
};

const originalWarn = console.warn;
console.warn = (...args) => {
  if (args[0]?.includes("validateDOMNesting")) {
    console.trace("validateDOMNesting warning occurred:");
  }
  originalWarn(...args);
};

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: true, // Revalidate when window gains focus
      staleTime: 10 * 60 * 1000,   // Data stays fresh for 10 minutes
    },
  },
});

ReactDOM.render(
  <React.StrictMode>
    <ErrorBoundaryComponent>
      <QueryClientProvider client={queryClient}>
        <AlertDialogProvider>
          <DataProvider>
            <App />
          </DataProvider>
        </AlertDialogProvider>
      </QueryClientProvider>
    </ErrorBoundaryComponent>
  </React.StrictMode>,
  document.getElementById("root")
);

reportWebVitals();
