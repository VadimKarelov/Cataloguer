import {Typography} from "antd";
import BaseButtonComponent from "../BaseButtonComponent";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import React from "react";

/**
 * Свойства компонента CheckEfficiencyButtonComponent.
 */
interface CheckEfficiencyButtonComponentProps extends BaseStoreInjector {
}

/**
 * Компонент кнопки Проверить, открывающий свою модалку.
 */
const CheckEfficiencyButtonComponent: React.FC<CheckEfficiencyButtonComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const currentBrochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Свойства модалки.
     */
    const modalProps = {
        title: "Проверить каталог",
        okText: "Проверить",
        cancelText: "Отменить",
        children: (<Typography.Text>{`Вы действительно хотите проверить эффективность каталога ${currentBrochure?.name}?`}</Typography.Text>),
    };

    /**
     * Свойства для кнопки, открывающей модальное окно.
     */
    const buttonProps = {
        buttonText: "Проверить",
        isDisabled: currentBrochure === null,
    };

    return (
        <BaseButtonComponent
            buttonProps={buttonProps}
            modalProps={modalProps}
        />
    );
}));

export default CheckEfficiencyButtonComponent;