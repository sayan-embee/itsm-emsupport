
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

export function handleMobileNumberKeyPress(event: React.KeyboardEvent<HTMLInputElement>): void {
    const key = event.key;

    // Allow control keys like backspace, delete, arrow keys, etc.
    if (["Backspace", "Tab", "ArrowLeft", "ArrowRight", "Delete"].includes(key)) {
        return;
    }

    const allowedCharsPattern = /^\+?[0-9]+$/;

    if (!allowedCharsPattern.test(key)) {
        event.preventDefault();
    }
}