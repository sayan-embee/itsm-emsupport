//#region Error Handlers

import { CommonMessage } from "../components/common/ToastComponent";
import { IClientInfo } from "../Interfaces";

export function handleErrorHelper(methodName: string, error: any): { statusCode: number; errorMessage: string } {
    console.error(`Error at: ${methodName}: `, error);

    let errorMessage = CommonMessage.Error.toString();
    let statusCode = 0;

    if (error.response) {
        statusCode = error.response.status;

        switch (statusCode) {
            case 401:
                errorMessage = CommonMessage.SessionExpired.toString();
                break;
            case 403:
                errorMessage = CommonMessage.Unauthorized.toString();
                break;
            case 500:
                errorMessage = CommonMessage.InternalServerError.toString();
                break;
            default:
                errorMessage = error.response.data?.message || `Error ${statusCode}: ${error.response.statusText}`;
                break;
        }
    }
    else if (error.request) {
        errorMessage = CommonMessage.ServerNotResponding.toString();
    }
    else {
        errorMessage = error.message;
    }

    return { statusCode, errorMessage };
};

//#endregion



//#region OnKeyPress Methods

export function handleTextKeyPress(event: React.KeyboardEvent<HTMLInputElement>): void {
    const key = event.key;

    // Allow control keys like backspace, delete, arrow keys, etc.
    if (["Backspace", "Tab", "ArrowLeft", "ArrowRight", "Delete"].includes(key)) {
        return;
    }

    const vulnerableCharsPattern = /[<>#\\\/;^~$!=]/;

    if (vulnerableCharsPattern.test(key)) {
        event.preventDefault();
    }
}

export function handleEmailKeyPress(event: React.KeyboardEvent<HTMLInputElement>): void {
    const key = event.key;

    // Allow control keys like backspace, delete, arrow keys, etc.
    const allowedControlKeys = ["Backspace", "Tab", "ArrowLeft", "ArrowRight", "Delete"];
    if (allowedControlKeys.includes(key)) {
        return;
    }

    // Allow keyboard shortcuts (e.g., Ctrl + A, Ctrl + C, Ctrl + V)
    if (event.ctrlKey || event.metaKey) {
        return;
    }

    const invalidEmailCharsPattern = /[<>#\\\/;^~$!=\s,]/;

    // Prevent invalid characters from being entered
    if (invalidEmailCharsPattern.test(key)) {
        event.preventDefault();
    }
}

export function handleAlphanumericKeyPress(event: React.KeyboardEvent<HTMLInputElement>): void {
    const key = event.key;

    // Allow control keys like backspace, delete, arrow keys, etc.
    if (["Backspace", "Tab", "ArrowLeft", "ArrowRight", "Delete"].includes(key)) {
        return;
    }

    const allowedCharsPattern = /^[A-Za-z0-9]$/;

    if (!allowedCharsPattern.test(key)) {
        event.preventDefault();
    }
}

export function handleNumericKeyPress(event: React.KeyboardEvent<HTMLInputElement>): void {
    const key = event.key;

    // Allow control keys like backspace, delete, arrow keys, etc.
    if (["Backspace", "Tab", "ArrowLeft", "ArrowRight", "Delete", "Enter"].includes(key) ||
        (event.ctrlKey && (key === 'c' || key === 'v'))) {
        return;
    }

    // Allow only numeric characters (0-9)
    const allowedNumbersPattern = /^[0-9]$/;

    if (!allowedNumbersPattern.test(key)) {
        event.preventDefault();
    }
}

export function handleNumericCommaKeyPress(event: React.KeyboardEvent<HTMLInputElement>): void {
    const key = event.key;

    // Allow control keys like backspace, delete, arrow keys, etc.
    if (["Backspace", "Tab", "ArrowLeft", "ArrowRight", "Delete", "Enter"].includes(key) ||
        (event.ctrlKey && (key === 'c' || key === 'v'))) {
        return;
    }

    // Allow only numeric characters (0-9) and comma (,)
    const allowedPattern = /^[0-9,]$/;

    if (!allowedPattern.test(key)) {
        event.preventDefault();
    }
}

//#endregion

export function IsNullOrEmpty(paramItem: string | null | undefined | any[]): boolean {
    try {
        if (paramItem === null || paramItem === undefined) {
            return true;
        }

        // Checks for empty strings, including those with only whitespace
        if (typeof paramItem === 'string') {
            return paramItem.trim() === '';
        }

        // Checks if the array is empty
        if (Array.isArray(paramItem)) {
            return paramItem.length === 0;
        }

        // For any other type return false
        return false;
    }
    catch (error) {
        console.error(`Error at IsNullOrEmpty()`, error);
        return false;
    }
}

export function UseRegex(regExp: RegExp, Value: string): boolean {
    try {
        return regExp.test(Value);
    }
    catch (error) {
        console.error(`Error at UseRegex()`, error);
        return false;
    }
}

export function MaskText(key: string, value: string, visibleCount: number): string {
    if (key.length !== 1) {
        throw new Error("Key must be a single character.");
    }

    if (visibleCount < 0 || visibleCount > value.length) {
        throw new Error("Visible count must be between 0 and the length of the value.");
    }

    const charArray = value.split('');
    const totalCount = charArray.length;

    const visibleIndices = new Set<number>();

    while (visibleIndices.size < visibleCount) {
        const randomIndex = Math.floor(Math.random() * totalCount);
        visibleIndices.add(randomIndex);
    }

    const maskedArray = charArray.map((char, index) => {
        return visibleIndices.has(index) ? char : key;
    });

    return maskedArray.join('');
}

export function MaskEmail(email: string): string {
    try {
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailPattern.test(email)) {
            throw new Error("Invalid email format.");
        }

        const [localPart, domainPart] = email.split('@');

        const visibleCount = 4;
        const maskedLocalPart = localPart.length > visibleCount
            ? `${localPart.slice(0, visibleCount - 1)}${'*'.repeat(localPart.length - visibleCount + 1)}${localPart[localPart.length - 1]}`
            : localPart;

        const [domainName, tld] = domainPart.split('.');
        const maskedDomainPart = `${domainName}${'.'.repeat(tld.length)}`;

        return `${maskedLocalPart}@${maskedDomainPart}`;
    }
    catch (error) {
        console.error(`Error at MaskEmail()`, error);
        return email;
    }
}

export async function GetClientInfo(): Promise<IClientInfo> {
    return new Promise((resolve) => {
        try {
            const userAgent = getBrowserName();
            const deviceType = detectDeviceType();

            // Default location values
            let latitude: number | null = null;
            let longitude: number | null = null;

            // Try to get geolocation
            // if ("geolocation" in navigator) {
            //     navigator.geolocation.getCurrentPosition(
            //         (position) => {
            //             latitude = position.coords.latitude ?? null;
            //             longitude = position.coords.longitude;
            //             resolve({ ClientIP: null, UserAgent: userAgent, DeviceType: deviceType, Location: { latitude, longitude } });
            //         },
            //         () => {
            //             // Location permission denied or error occurred
            //             resolve({ ClientIP: null, UserAgent: userAgent, DeviceType: deviceType, Location: { latitude, longitude } });
            //         }
            //     );
            // } else {
            //     resolve({ ClientIP: null, UserAgent: userAgent, DeviceType: deviceType, Location: { latitude, longitude } });
            // }

            resolve({ ClientIP: null, UserAgent: userAgent, DeviceType: deviceType, Location: { latitude, longitude } });

        } catch (error) {
            console.error("Error at GetClientInfo():", error);
            resolve({
                ClientIP: null,
                UserAgent: "Unknown",
                DeviceType: "Unknown",
                Location: { latitude: null, longitude: null }
            });
        }
    });
}

function detectDeviceType(): string {
    const userAgent = navigator.userAgent;
    if (/Mobile|Android|iPhone|iPad/i.test(userAgent)) return "Mobile";
    if (/Mac|Windows|Linux/i.test(userAgent)) return "Desktop";
    return "Unknown";
}

export function getBrowserName(): string {
    const userAgent = navigator.userAgent;

    if (userAgent.includes("Edg/")) return "Microsoft Edge";
    if (userAgent.includes("Chrome/") && userAgent.includes("Safari/")) return "Google Chrome";
    if (userAgent.includes("Safari/") && !userAgent.includes("Chrome/")) return "Apple Safari";
    if (userAgent.includes("Firefox/")) return "Mozilla Firefox";
    if (userAgent.includes("MSIE") || userAgent.includes("Trident/")) return "Internet Explorer";

    return "Unknown";
}


//#region Style Methods

export function GetStatusClassStyle(statusId: number): string {
    switch (statusId) {
        case 1:
            return 'default';
        case 2:
            return 'open';
        case 3:
            return 'default';
        case 4:
            return 'default';
        case 5:
            return 'close';

        default:
            return 'default';
    }
}

//#endregion