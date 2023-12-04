import React from "react";
import { ReactComponent as EffectiveStatusIcon  } from '../assets/successful-status.svg';
import { ReactComponent as IneffectiveStatusIcon  } from '../assets/ineffective-status.svg';
import { ReactComponent as UncheckedStatusIcon  } from '../assets/unchecked-status.svg';
import {Status, BROCHURE_STATUSES} from "../stores/BrochureStore";
import "../styles/StatusIconsComponent.css";

/**
 * Компонент, возвращающий иконку по статусу.
 */
const StatusIconsComponent: React.FC<{status: string, potentialIncome: number}> = (props) => {
    /**
     * Возвращает иконку, отражающую статус каталога.
     */
    const getStatusIcon = () => {
        const statusGettingAttempt = BROCHURE_STATUSES.find(status => status.value === props.status);
        switch (statusGettingAttempt?.key) {
            case Status.RELEASED: return (<EffectiveStatusIcon/>);
            case Status.DRAFT: {
                if (props.potentialIncome > 0) {
                    return (<IneffectiveStatusIcon/>);
                }
                return (<UncheckedStatusIcon/>);
            }
            default: return null;
        }
    };

    return (
        <div className={"brochure-menu-icons-style"}>
            {getStatusIcon()}
        </div>
    );
};

export default StatusIconsComponent;