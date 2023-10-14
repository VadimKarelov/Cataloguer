import React, {useEffect, useState} from "react";
import {Menu, Space, Spin} from "antd";
import type { MenuProps } from 'antd';
import { MailOutlined } from '@ant-design/icons';
import "../styles/BrochureMenu.css";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector} from "../types/BrochureTypes";

/**
 * Псевдотип, отражающий возможные статусы каталога.
 */
type BrochureStatusType = "effective" | "ineffective" | "unchecked";

/**
 * Свойства компонента BrochureMenuComponent.
 * @property brochureStore Хранилище данных по каталогам.
 */
interface BrochureMenuComponentProps extends BaseStoreInjector {
}

/**
 * Компонент меню каталогов.
 */
const BrochureMenuComponent: React.FC<BrochureMenuComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Возвращает иконку, отражающую статус каталога.
     * Пока только один статус.
     * @param status Статус каталога.
     */
    const getStatusIcon = (status: BrochureStatusType = "unchecked") => {
        switch (status) {
            default: return (<MailOutlined />);
        }
    };

    /**
     * Возвращает компонент span для элемента меню.
     * @param name Название каталога.
     */
    const getMenuItemSpan = (name: string) => {
        return (
            <span className={"brochure-menu-items-style"}>
                <Space>
                    {getStatusIcon()}{name}
                </Space>
            </span>
        );
    };

    /**
     * Хук для хранения коллекции элементов меню.
     */
    const [brochureItems, setBrochureItems] = useState<MenuProps['items']>([]);

    /**
     * Загружает каталог из настроек браузера.
     */
    const loadSelectedBrochure = (): void => {
        const preBrochure = props.brochureStore?.getSavedBrochure() ?? null;
        const id = preBrochure !== null ? preBrochure.id : -1;
        if (id === -1) return;

        onSelectBrochure({key: `brochure_${id}`});
    };

    /**
     * Хук, вызывающий запрос на загрузку каталогов при монтировании компонента в DOM.
     * Загружает каталоги из БД.
     */
    useEffect(() => {
        props.brochureStore?.loadBrochures();
        loadSelectedBrochure();
    }, []);

    /**
     * Хук, необходимый для преобразования каталогов и элементы, подходящие под компонент Menu.
     */
    useEffect(() => {
        const brochures = props.brochureStore?.brochures ?? [];
        setBrochureItems(
            brochures.map(brochure => {
                return ({
                    label: getMenuItemSpan(`Каталог ${brochure.id}`),
                    key: `brochure_${brochure.id}`,
                });
            })
        );
    }, [props.brochureStore?.brochures]);

    /**
     * Изменяет выбранный каталог.
     * @param event Событие, содержащее данные по каталогу.
     */
    const onSelectBrochure = (event: {key: string}): void => {
        props.brochureStore?.reset();
        console.log(event)
        const brochureKey: string = event.key ?? "";
        const id = brochureKey.slice(brochureKey.indexOf('_') + 1);
        props.brochureStore?.onBrochureClick(id !== "undefined" ? parseInt(id) : -1);
    };

    /**
     * Выбранные элементы меню.
     */
    const selectedMenuItems: Readonly<string[]> = props.brochureStore?.getSavedBrochureMenu() ?? [];

    return (
        <Spin spinning={props.brochureStore?.isBrochureMenuLoading}>
            <Menu
                className={"brochure-menu-style"}
                mode={"inline"}
                defaultSelectedKeys={selectedMenuItems.map(item => item)}
                items={brochureItems}
                onClick={onSelectBrochure}
            />
        </Spin>
    );
}));

export default BrochureMenuComponent;