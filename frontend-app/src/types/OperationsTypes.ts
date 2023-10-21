import React from "react";

/**
 * Свойства модального окна.
 * @param buttonText Текст кнопки.
 * @param onClick Калбек функция при нажатии на кнопку.
 */
interface ButtonProps {
    buttonText: string,
    onClick?: () => void,
}

/**
 * Свойства модального окна.
 * @param okText Текст на кнопке ок.
 * @param cancelText Текст на кнопке отмена.
 * @param title Заголовок модального окна.
 * @param children Дочерние компоненты.
 */
interface ModalProps {
    okText?: string,
    cancelText?: string,
    title: React.ReactNode,
    children: React.JSX.Element,
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