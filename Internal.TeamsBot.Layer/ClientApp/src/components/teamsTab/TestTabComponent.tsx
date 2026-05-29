import React from "react";
import { useAuth } from "../../components/auth/AuthProvider";
import { Message } from "primereact/message";

const TestTabComponent: React.FC = () => {
    const { teamsSSOToken, teamsSSOError } = useAuth();

    return (
        <div className="your-component-container">
            {teamsSSOError ? (
                <div className="d-flex justify-content-center align-items-center vh-100">
                    < Message severity="error" text={teamsSSOError} />
                </div>
            ) : (
                teamsSSOToken && (
                    <div className="d-flex justify-content-center align-items-center vh-100">
                        <Message severity="success" text="Authenticated" />
                        <br />
                        <p>{teamsSSOToken.name} ({teamsSSOToken.upn})</p>
                    </div>
                )
            )
            }
        </div>
    );
};

export default TestTabComponent;