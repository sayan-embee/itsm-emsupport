import { useState } from "react";
import { useHistory } from "react-router-dom";
import { Switch, Route, Redirect } from "react-router-dom";
import { Button } from "primereact/button";
import { Routes } from "../../router";
import { Tooltip } from "primereact/tooltip";

const Home = () => {
    const [collapsed, setCollapsed] = useState(false);
    const history = useHistory();

    return (
        <div className="d-flex vh-100">

            {/* Sidebar */}
            <div
                className="bg-light border-end transition-all d-flex flex-column"
                style={{
                    width: collapsed ? "3rem" : "12rem",
                    minWidth: collapsed ? "3rem" : "12rem",
                    overflow: "hidden"
                }}
            >
                {/* Toggle Button */}
                <div className="sidebar-toggle d-flex align-items-center"
                    onClick={() => setCollapsed(!collapsed)}
                >
                    <Button
                        icon={collapsed ? 'pi pi-bars' : 'pi pi-chevron-left'}
                        onClick={() => setCollapsed(!collapsed)}
                        className="m-2 p-button-sm p-button-text"
                    />
                    {
                        !collapsed && (
                            <span>Menu</span>
                        )
                    }
                </div>

                {/* Menu */}
                <ul className="list-unstyled m-0 p-2 flex-grow-1 sidebar-menu">
                    {Routes.filter(r => r.name).map((route, i) => (
                        <li
                            key={route.path}
                            onClick={() => history.push(route.path)}
                            className={`d-flex align-items-center mb-2 p-2 rounded ${route.path === history.location.pathname ? "active" : ""}`}
                            data-pr-tooltip={route.name}
                            data-pr-position="right"
                        >
                            <i className={`pi ${route.icon || "pi-angle-right"}`} />
                            {!collapsed && <span className="ms-2">{route.name}</span>}
                        </li>
                    ))}
                </ul>
            </div>

            {/* Main Content */}
            <div className="flex-grow-1 overflow-auto">
                <Switch>
                    {Routes.map((route) =>
                        route.redirectTo ? (
                            <Redirect key={route.path} to={route.redirectTo} />
                        ) : (
                            <Route
                                key={route.path}
                                exact={route.exact}
                                path={route.path}
                                component={route.component}
                            />
                        )
                    )}
                </Switch>
            </div>

            {/* Tooltip for collapsed menu */}
            {collapsed && <Tooltip target="[data-pr-tooltip]" />}
        </div>
    );
};

export default Home;