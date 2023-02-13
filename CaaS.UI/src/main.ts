import {DEFAULT_CURRENCY_CODE, enableProdMode, LOCALE_ID} from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import {getLocaleCurrencyCode} from "@angular/common";

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule).catch(err => console.error(err));
