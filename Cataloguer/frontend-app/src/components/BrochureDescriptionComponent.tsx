import { Result } from "antd";
import React from "react";

/**
 * Компонент описания каталога.
 */
const BrochureDescriptionComponent: React.FC = () => {
    return (
        <Result
            status="warning"
            title="Раздел 'Каталог' находится в разработке."
            // extra={
            //     <Button type="primary" key="console">
            //         Go Console
            //     </Button>
            // }
        />
    );
};

export default BrochureDescriptionComponent;