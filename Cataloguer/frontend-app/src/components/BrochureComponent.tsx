import React from "react";
import {Empty, Spin} from "antd";
import {inject, observer} from "mobx-react";
import BrochureWorkingAreaComponent from "./BrochureWorkingAreaComponent";
import {Content} from "antd/es/layout/layout";
import {BaseStoreInjector} from "../types/BrochureTypes";

/**
 * Свойства компонента BrochureComponent.
 */
interface BrochureComponentProps extends BaseStoreInjector {
}

/**
 * Основной компонент каталога.
 */
const BrochureComponent: React.FC<BrochureComponentProps> = inject("brochureStore")(observer((props) => {
    /**
     * Выбранный каталог.
     */
    const currentBrochure = props.brochureStore?.currentBrochure ?? null;

    return (
        <Spin size={"large"} spinning={props.brochureStore?.isBrochureMenuLoading || props.brochureStore?.isBrochureLoading}>
            <Content className={"brochure-content-style"}>
                {
                    (currentBrochure !== null) ?
                        <BrochureWorkingAreaComponent/>
                    : <Empty image={Empty.PRESENTED_IMAGE_SIMPLE}/>
                }
            </Content>
        </Spin>
    );
}));

export default BrochureComponent;