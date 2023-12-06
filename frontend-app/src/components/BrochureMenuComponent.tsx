import React, {useEffect, useState} from "react";
import {Empty, Menu, Space, Spin} from "antd";
import type { MenuProps } from 'antd';
import "../styles/BrochureMenu.css";
import {inject, observer} from "mobx-react";
import {BaseStoreInjector} from "../types/BrochureTypes";
import StatusIconsComponent from "./IconsComponent";

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
     * Возвращает компонент span для элемента меню.
     * @param name Название каталога.
     * @param status Статус каталога.
     * @param potentialIncome Потенциальный доход с каталога, характеризующий эффективность.
     */
    const getMenuItemSpan = (name: string, status: string, potentialIncome: number) => {
        return (
            <span className={"brochure-menu-items-style"}>
                <Space>
                    <StatusIconsComponent status={status} potentialIncome={potentialIncome}/>{name}
                </Space>
            </span>
        );
    };

    /**
     * Хук, необходимый для преобразования каталогов и элементы, подходящие под компонент Menu.
     */
    useEffect(() => {
        const brochures = props.brochureStore?.brochures ?? [];
        setBrochureItems(
            brochures.map(brochure => {
                return ({
                    label: getMenuItemSpan(
                        brochure.name ?? `Каталог ${brochure.id}`,
                        brochure.statusName,
                        brochure.potentialIncome
                    ),
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

        const brochureKey: string = event.key ?? "";
        const id = brochureKey.slice(brochureKey.indexOf('_') + 1);
        props.brochureStore?.onBrochureClick(id !== "undefined" ? parseInt(id) : -1);
    };

    /**
     * Выбранные элементы меню.
     */
    const [currentlySelectedMenuItems, setCurrentlySelectedMenuItems] = useState<string[]>([]);

    /**
     * Хук, обновляющий выбранный элемент в меню.
     */
    useEffect(() => {
        setCurrentlySelectedMenuItems(props.brochureStore?.getSavedBrochureMenu() ?? []);
    }, [props.brochureStore?.currentBrochure]);

    return (
        <Spin className={"white-background-style"} spinning={props.brochureStore?.isBrochureMenuLoading}>
            {brochureItems && brochureItems.length > 0 ?
                <Menu
                    className={"brochure-menu-style"}
                    mode={"inline"}
                    selectedKeys={currentlySelectedMenuItems}
                    items={brochureItems}
                    onClick={onSelectBrochure}
                />
            : <Empty
                className={"empty-menu-style"}
                image={Empty.PRESENTED_IMAGE_SIMPLE}
                description={"Нет каталогов"}
            />}
        </Spin>
    );
}));

export default BrochureMenuComponent;