import React from "react";
import {Empty, Layout, Spin} from "antd";
import {inject, observer} from "mobx-react";
import BrochureWorkingAreaComponent from "./BrochureWorkingAreaComponent";
import {Content} from "antd/es/layout/layout";
import {BaseStoreInjector} from "../types/BrochureTypes";
import Sider from "antd/es/layout/Sider";
import BrochureMenuComponent from "./BrochureMenuComponent";

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
        <Layout className={"white-background-style"}>
            <Sider width={350} className={"sider-style"}>
                <BrochureMenuComponent/>
            </Sider>
            <Content className={"brochure-content-style white-background-style"}>
                <Spin size={"large"} spinning={props.brochureStore?.isBrochureMenuLoading || props.brochureStore?.isBrochureLoading}>
                    {
                        (currentBrochure !== null) ?
                            <BrochureWorkingAreaComponent/>
                        : <Empty image={Empty.PRESENTED_IMAGE_SIMPLE}/>
                    }
                </Spin>
            </Content>
        </Layout>
    );
}));

export default BrochureComponent;