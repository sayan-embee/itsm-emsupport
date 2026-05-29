import ChannelConfigComponent from "./components/config/ChannelConfigComponent";
import ReportTabComponent from "./components/teamsTab/ReportTabComponent";
import ContractMasterCreateComponent from "./components/teamsTab/ContractMasterCreateComponent";
import ContractMasterListComponent from "./components/teamsTab/ContractMasterListComponent";
import ContractMasterViewComponent from "./components/teamsTab/ContractMasterViewComponent";
import ErrorComponent from "./components/common/ErrorComponent";
// import ContractMasterListComponentV2 from "./components/teamsTab/ContractMasterListComponentV2";
// import WebChatComponent from "./components/externalTabs/WebChatComponent";
// import TestTabComponent from "./components/teamsTab/TestTabComponent";

interface IMSTab {
  websiteUrl: string,
  contentUrl: string,
  entityId: string,
  suggestedDisplayName: string
}

export const MS_TABS: Record<string, IMSTab> = {
  ReportTab: {
    websiteUrl: window.location.origin,
    contentUrl: window.location.origin + '/report',
    entityId: "reportTab",
    suggestedDisplayName: "Report"
  },
};

export const Routes = [
  // { path: "/channelConfig", component: ChannelConfigComponent },
  { path: "/report", exact: false, name: 'Monthly Report', icon: 'pi pi-file', component: ReportTabComponent },
  { path: "/newContract", exact: false, name: '', component: ContractMasterCreateComponent },
  { path: "/listContract", exact: false, name: 'Manage Contracts', icon: 'pi pi-list', component: ContractMasterListComponent },
  { path: "/viewContract", exact: false, name: '', icon: '', component: ContractMasterViewComponent },
  { path: "/error", exact: true, name: '', icon: '', component: ErrorComponent },
  // { path: "/", exact: true, component: <ErrorComponent message="Something went wrong. Please refresh the application." severity="warn" /> }
  { path: "/", exact: true, name: '', redirectTo: '/report' }
];