import { useEffect, useRef, useState } from "react";
import { OverlayPanel } from "primereact/overlaypanel";
import { Tag } from "primereact/tag";
import useIsMobile from "./useIsMobile";

interface VersionInfo {
    slNo: number;
    version: string;
    enableOverlay?: boolean;
    releasedOn_header?: string;
    releasedOn?: string;
    notes_header?: string;
    notes?: string[];
}

const AppVersion = () => {
    const [latest, setLatest] = useState<VersionInfo | null>(null);
    const op = useRef<OverlayPanel>(null);

    const isMobile = useIsMobile();

    useEffect(() => {
        fetch("/versions.json")
            .then((res) => res.json())
            .then((data: VersionInfo[]) => {
                console.log('Version data: ', data);
                const maxVersion = data.reduce((prev, current) =>
                    current.slNo > prev.slNo ? current : prev
                );
                setLatest(maxVersion);
            });
    }, []);

    if (!latest) return null;

    return (
        <>
            <Tag
                value={`v${latest.version}`}
                severity="info"
                style={{
                    cursor: "pointer",
                }}
                // onMouseEnter={(e) => op.current?.show(e, e.currentTarget)}
                onMouseEnter={(e) => {
                    if (!isMobile) {
                        op.current?.show(e, e.currentTarget);
                    }
                }}
                onClick={(e) => {
                    if (isMobile) {
                        op.current?.toggle(e, e.currentTarget)
                    }
                }}
            // onMouseLeave={() => op.current?.hide()}
            />
            {
                latest.enableOverlay && (
                    <OverlayPanel ref={op} dismissable>
                        <div className="text-sm">
                            {latest.releasedOn && (
                                <div className="mb-2">
                                    <p className="m-0" style={{ fontSize: '12px' }}><strong>{latest.releasedOn_header}</strong> {latest.releasedOn}</p>
                                </div>
                            )}
                            {latest.notes && (
                                <div>
                                    <p className="m-0" style={{ fontSize: '12px' }}><strong>{latest.notes_header}</strong></p>
                                    <ul className="ms-3 mb-0">
                                        {latest.notes.map((note, idx) => (
                                            <li style={{ fontSize: '12px' }} key={idx}>{note}</li>
                                        ))}
                                    </ul>
                                </div>
                            )}
                        </div>
                    </OverlayPanel>
                )
            }
        </>
    );
};

export default AppVersion;