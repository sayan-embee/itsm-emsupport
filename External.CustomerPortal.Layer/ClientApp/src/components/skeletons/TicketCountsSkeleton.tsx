import React from 'react';
import { Skeleton } from 'primereact/skeleton';
import Carousel from 'react-multi-carousel';
import { Button } from 'primereact/button';

import logo from '../../assets/Logo.svg';
import alltkt from '../../assets/alltkt_lg.svg';
import closetkt from '../../assets/closetkt_lg.svg';
import opentkt from '../../assets/opentkt_lg.svg';
import resonse from '../../assets/resonse-icon.svg';
import resolution from '../../assets/resolution-icon.svg';
import { ProgressSpinner } from 'primereact/progressspinner';


const TicketCountsSkeleton: React.FC = () => {

    const responsive: any = {
        desktop: {
            breakpoint: {
                max: 3000,
                min: 1024
            },
            items: 6,
            slidesToSlide: 1,
            partialVisibilityGutter: 40,
            visible: 6,
        },
        mobile: {
            breakpoint: {
                max: 464,
                min: 0
            },
            items: 1,
            slidesToSlide: 2,
            partialVisibilityGutter: 30
        },
        tablet: {
            breakpoint: {
                max: 1024,
                min: 200
            },
            items: 1,
            slidesToSlide: 1,
            partialVisibilityGutter: 30
        }
    };

    return (
        <div className='mb-3 mb-lg-0'>
            <Carousel className="dash-carousel" responsive={responsive} showDots={true} infinite={true} autoPlay={true} containerClass="carousel-with-custom-dots" autoPlaySpeed={3000} removeArrowOnDeviceType={["tablet", "mobile"]}>
                <div className='item'>
                    <div className="dash-stats-card alltkt">
                        <div>
                            <div className="d-flex align-items-start">
                                <img className="icon" src={alltkt} alt="" />
                                <div>
                                    <h2 className="text-blue m-0"><ProgressSpinner style={{ width: '25px', height: '25px' }} strokeWidth="1" /></h2>
                                </div>
                            </div>
                            <p className="mb-2 mt-2">All Tickets This Month</p>
                        </div>
                        <div className='d-flex justify-content-between'>
                            {/* <div className="d-flex align-items-center text-xs status-move">
                                <i className="pi pi-arrow-up-right"></i>
                                <span className="text-xs">+1.01% this week</span>
                            </div> */}
                            <div className="d-flex align-items-end">
                                <Button icon="pi pi-arrow-right" rounded />
                            </div>
                        </div>

                    </div>
                </div>
                <div className='item'>
                    <div className="dash-stats-card closetkt">
                        <div>
                            <div className="d-flex align-items-start">
                                <img className="icon" src={closetkt} alt="" />
                                <div>
                                    <h2 className="text-blue m-0"><ProgressSpinner style={{ width: '25px', height: '25px' }} strokeWidth="1" /></h2>
                                </div>
                            </div>
                            <p className="mb-2 mt-2">Closed Tickets This Month</p>
                        </div>
                        <div className='d-flex justify-content-between'>
                            {/* <div className="d-flex align-items-center text-xs status-move">
                                <i className="pi pi-arrow-up-right"></i>
                                <span className="text-xs">+1.01% this week</span>
                            </div> */}
                            <div className="d-flex align-items-end">
                                <Button icon="pi pi-arrow-right" rounded />
                            </div>
                        </div>

                    </div>
                </div>
                <div className='item'>
                    <div className="dash-stats-card opentkt">
                        <div>
                            <div className="d-flex align-items-start">
                                <img className="icon" src={opentkt} alt="" />
                                <div>
                                    <h2 className="text-blue m-0"><ProgressSpinner style={{ width: '25px', height: '25px' }} strokeWidth="1" /></h2>
                                </div>
                            </div>
                            <p className="mb-2 mt-2">Open Tickets This Month</p>
                        </div>
                        <div className='d-flex justify-content-between'>
                            {/* <div className="d-flex align-items-center text-xs status-move">
                                <i className="pi pi-arrow-down-left"></i>
                                <span className="text-xs">+1.01% this week</span>
                            </div> */}
                            <div className="d-flex align-items-end">
                                <Button icon="pi pi-arrow-right" rounded />
                            </div>
                        </div>

                    </div>
                </div>
                {/* <div className='item'>
                    <div className="dash-stats-card response">
                        <div>
                            <div className="d-flex align-items-start">
                                <img className="icon" src={resonse} alt="" />
                                <div>
                                    <h2 className="text-blue m-0"><ProgressSpinner style={{ width: '25px', height: '25px' }} strokeWidth="1" /></h2>
                                </div>
                            </div>
                            <p className="mb-2 mt-2">SLA Violated (Response)</p>
                        </div>
                        <div className="d-flex justify-content-between">
                            <div className="d-flex align-items-center text-xs status-move">
                                <i className="pi pi-arrow-down-left"></i>
                                <span className="text-xs">+1.01% this week</span>
                            </div>
                            <div className="d-flex align-items-end">
                                <Button icon="pi pi-arrow-right" rounded />
                            </div>
                        </div>
                    </div>
                </div> */}
                {/* <div className='item'>
                    <div className="dash-stats-card resolution">
                        <div>
                            <div className="d-flex align-items-start">
                                <img className="icon" src={resolution} alt="" />
                                <div>
                                    <h2 className="text-blue m-0"><ProgressSpinner style={{ width: '25px', height: '25px' }} strokeWidth="1" /></h2>
                                </div>
                            </div>
                            <p className="mb-2 mt-2">SLA Violated (Resolution)</p>
                        </div>
                        <div className="d-flex justify-content-between">
                            <div className="d-flex align-items-center text-xs status-move">
                                <i className="pi pi-arrow-down-left"></i>
                                <span className="text-xs">+1.01% this week</span>
                            </div>
                            <div className="d-flex align-items-end">
                                <Button icon="pi pi-arrow-right" rounded />
                            </div>
                        </div>

                    </div>
                </div> */}
            </Carousel>
        </div>
    );
};

export default TicketCountsSkeleton;