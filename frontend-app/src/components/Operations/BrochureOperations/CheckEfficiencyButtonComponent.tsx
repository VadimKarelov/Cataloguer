import {Typography} from "antd";
import BaseButtonComponent from "../BaseButtonComponent";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import React from "react";
import {openNotification} from "../../NotificationComponent";

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
     * Возвращает настройки хинта.
     */
    const getTooltipProps = () => {
        const title = "Необходимо выбрать каталог";
        return {title: title};
    };

    /**
     * Вызывает метод запуска расчёта.
     */
    const onOkClick = () => {
        const response = props.brochureStore?.startBrochureRun();
        response?.then(
            (resolve: string) => {openNotification("Успех", resolve, "success")},
            (error: string) => {openNotification("Ошибка", error, "error")}
        );
        return Promise.resolve();
    };

    /**
     * Свойства модалки.
     */
    const modalProps = {
        title: "Проверить каталог",
        okText: "Проверить",
        cancelText: "Отменить",
        children: (<Typography.Text>{`Вы действительно хотите проверить эффективность каталога ${currentBrochure?.name}?`}</Typography.Text>),
        onOkClick: onOkClick,
    };

    /**
     * Свойства для кнопки, открывающей модальное окно.
     */
    const buttonProps = {
        buttonText: "Проверить",
        isDisabled: currentBrochure === null,
        tooltip: getTooltipProps(),
    };

    return (
        <BaseButtonComponent
            buttonProps={buttonProps}
            modalProps={modalProps}
        />
    );
}));

export default CheckEfficiencyButtonComponent;