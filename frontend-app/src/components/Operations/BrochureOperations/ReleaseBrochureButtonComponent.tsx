import {Typography} from "antd";
import BaseButtonComponent from "../BaseButtonComponent";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector} from "../../../types/BrochureTypes";
import React from "react";

/**
 * Свойства компонента ReleaseBrochureButtonComponent.
 */
interface ReleaseBrochureButtonComponentProps extends BaseStoreInjector {
}

/**
 * Компонент кнопки Выпустить, открывающий свою модалку.
 */
const ReleaseBrochureButtonComponent: React.FC<ReleaseBrochureButtonComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const currentBrochure = props.brochureStore?.currentBrochure ?? null;

    /**
     * Выбран ли каталог.
     */
    const isBrochureSelected = currentBrochure !== null;

    /**
     * Проверена ли эффективность каталога.
     */
    const isBrochureChecked = isBrochureSelected && currentBrochure!.potentialIncome > 0;

    /**
     * Возвращает настройки хинта.
     */
    const getTooltipProps = () => {
        let title = "Нет условий для ограничения";
        if (!isBrochureSelected) {
            title = "Необходимо выбрать каталог";
        } else if (!isBrochureChecked) {
            title = "Необходимо проверить каталог на эффективность";
        }
        return {title: title};
    };

    /**
     * Вызывает метод запуска расчёта.
     */
    const onOkClick = () => {
        props.brochureStore?.releaseBrochure();
        return Promise.resolve();
    };

    /**
     * Свойства модалки.
     */
    const modalProps = {
        title: "Выпустить каталог",
        okText: "Выпустить",
        cancelText: "Отменить",
        children: (<Typography.Text>{`Вы действительно хотите выпустить каталог ${currentBrochure?.name}?`}</Typography.Text>),
        onOkClick: onOkClick,
    };

    /**
     * Свойства для кнопки, открывающей модальное окно.
     */
    const buttonProps = {
        buttonText: "Выпустить",
        isDisabled: !isBrochureSelected || !isBrochureChecked,
        tooltip: getTooltipProps(),
    };

    return (
        <BaseButtonComponent
            buttonProps={buttonProps}
            modalProps={modalProps}
        />
    );
}));

export default ReleaseBrochureButtonComponent;