import React, { Component, ErrorInfo, ReactNode } from "react";
import accessDeniedImage from "../../assets/access_denied.jpg";
import { Message } from "primereact/message";
import { CommonMessage } from "./ToastComponent";

interface Props {
    children: ReactNode;
}

interface State {
    hasError: boolean;
}

class ErrorBoundaryComponent extends Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.state = { hasError: false };
    }

    static getDerivedStateFromError(_: Error) {
        return { hasError: true };
    }

    componentDidCatch(error: Error, errorInfo: ErrorInfo) {
        console.error("Error caught by boundary:", error, errorInfo);
    }

    render() {
        if (this.state.hasError) {
            <div className="overlay-container">
                <img className="error-image" src={accessDeniedImage} alt="Access Denied" />
                <div className="mt-3">
                    <Message severity='error' text={CommonMessage.InternalServerError} />
                </div>
            </div>
        }
        return this.props.children;
    }
}

export default ErrorBoundaryComponent;