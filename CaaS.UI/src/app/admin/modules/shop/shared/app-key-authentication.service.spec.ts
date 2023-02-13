import { TestBed } from '@angular/core/testing';

import { AppKeyAuthenticationService } from './app-key-authentication.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('AppKeyAuthenticationService', () => {
  let service: AppKeyAuthenticationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(AppKeyAuthenticationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
