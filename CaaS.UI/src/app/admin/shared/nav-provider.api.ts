import { INavData } from "@coreui/angular";

export abstract class NavProviderApi {
  abstract navItems(): Promise<Array<INavData>>;
}
