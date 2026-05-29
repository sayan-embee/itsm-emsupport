import React, { useEffect } from 'react';
import { app, pages } from "@microsoft/teams-js";
import { MS_TABS } from '../../router';

const ChannelConfigComponent: React.FC = () => {
    useEffect(() => {
        const initializeTeamsApp = async () => {
            try {
                // Initialize the Teams SDK
                await app.initialize();

                // Register the Save Handler
                pages.config.registerOnSaveHandler((saveEvent) => {
                    pages.config
                        .setConfig({
                            websiteUrl: MS_TABS.ReportTab.websiteUrl,
                            contentUrl: MS_TABS.ReportTab.contentUrl,
                            entityId: MS_TABS.ReportTab.entityId,
                            suggestedDisplayName: MS_TABS.ReportTab.suggestedDisplayName,
                        })
                        .then(() => {
                            saveEvent.notifySuccess();
                        })
                        .catch((error) => {
                            saveEvent.notifyFailure(error.message);
                        });
                });

                // Enable the Save button
                pages.config.setValidityState(true);
            } catch (error) {
                console.error("Error initializing Teams SDK:", error);
            }
        };

        initializeTeamsApp();
    }, []);

    return (
        <div className="configContainer">
            <h3>Please click the Save button to complete the process</h3>
        </div>
    );
};

export default ChannelConfigComponent;