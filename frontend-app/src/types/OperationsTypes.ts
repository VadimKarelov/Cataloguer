import React from "react";

/**
 * Свойства модального окна.
 * @param buttonText Текст кнопки.
 * @param onClick Калбек функция при нажатии на кнопку.
 * @param isDisabled Не активна кнопка или нет.
 * @param tooltip Свойства хинта для кнопки модалки.
 */
interface ButtonProps {
    buttonText: string,
    onClick?: () => void,
    isDisabled?: boolean,
    tooltip: TooltipProps,
}

/**
 * Свойства хинта для кнопки модалки.
 * @param title Сообщение в хинте.
 */
export interface TooltipProps {
    title: string,
}

/**
 * Свойства модального окна.
 * @param okText Текст на кнопке ок.
 * @param onOkClick Событие при нажитии кнопки ок.
 * @param onCancelClick Событие при нажитии кнопки отменить/крестик.
 * @param cancelText Текст на кнопке отмена.
 * @param title Заголовок модального окна.
 * @param children Дочерние компоненты.
 */
interface ModalProps {
    okText?: string,
    onOkClick?: () => Promise<void>,
    onCancelClick?: () => Promise<void>,
    cancelText?: string,
    title: React.ReactNode,
    children: React.JSX.Element | null,
}

/**
 * Свойства компонента BaseButtonComponent.
 * @param buttonProps Свойства кнопки.
 * @param modalProps Свойства модального окна.
 */
interface BaseButtonComponentProps {
    buttonProps: ButtonProps,
    modalProps: ModalProps,
}

export type {
    ButtonProps,
    ModalProps,
    BaseButtonComponentProps,
}