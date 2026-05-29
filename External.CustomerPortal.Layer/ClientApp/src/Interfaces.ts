// Interfaces

export interface ITeamsToken {
    aud: string; // Audience
    iss: string; // Issuer
    iat: number; // Issued At
    nbf: number; // Not Before
    exp: number; // Expiration
    acr: string; // Authentication Context Class Reference
    aio: string; // AIO
    amr: string[]; // Authentication Methods Reference
    appid: string; // Application ID
    appidacr: string; // Application ID Authentication Context Class Reference
    family_name: string; // Family Name
    given_name: string; // Given Name
    ipaddr: string; // IP Address
    name: string; // Name
    oid: string; // Object ID
    rh: string; // Refresh Token
    scp: string; // Scope
    sub: string; // Subject
    tid: string; // Tenant ID
    unique_name: string; // Unique Name
    upn: string; // User Principal Name
    uti: string; // Unique Token Identifier
    ver: string; // Version
}

export interface IAxiosResponse {
    config: any;
    data: any;
    headers: any;
    status: number;
    statusText: string;
}

export interface IState {
    Value: any;
    IsRequired: boolean;
    IsValid: boolean;
    IsDisabled?: boolean;
    IsLoading?: boolean;
    ErrorMessage: string;
}

export interface IDropdownOption {
    code: any;
    name: string;
    info?: any;
}

export interface IClientInfo {
    ClientIP: any;
    UserAgent: any;
    DeviceType: any;
    Location: {
        latitude: number | null;
        longitude: number | null;
    };
}