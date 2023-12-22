import {Typography} from "antd";
import BaseButtonComponent from "../BaseButtonComponent";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import React, {useEffect} from "react";
import {openNotification} from "../../NotificationComponent";
import {DistributionStore} from "../../../stores/DistributionStore";

/**
 * Свойства компонента CheckEfficiencyButtonComponent.
 */
interface CheckEfficiencyButtonComponentProps extends BaseStoreInjector {
    distributionStore?: DistributionStore;
}

/**
 * Компонент кнопки Проверить, открывающий свою модалку.
 */
const CheckEfficiencyButtonComponent: React.FC<CheckEfficiencyButtonComponentProps> = inject("brochureStore", "distributionStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const currentBrochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Выбран каталог или нет.
     */
    const isBrochureSelected = currentBrochure !== null;

    /**
     * Есть рассылки или нет.
     */
    const hasDistributions = (props.distributionStore?.distributions ?? []).length > 0;

    /**
     * Обновляет число рассылок для текущего каталога.
     */
    useEffect(() => {
        if (currentBrochure) {
            props.distributionStore?.updateBrochureDistributions(currentBrochure?.id);
        }
    }, [currentBrochure?.id]);

    /**
     * Возвращает настройки хинта.
     */
    const getTooltipProps = () => {
        let title = "Необходимо выбрать каталог";
        if (isBrochureSelected && !hasDistributions) {
            title = "Необходимо добавить рассылки";
        }
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
        isDisabled: !isBrochureSelected || !hasDistributions,
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