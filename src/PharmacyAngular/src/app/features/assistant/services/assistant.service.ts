import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AiInventoryChatRequest } from '../models/ai-inventory-chat-request';
import { AiInventoryChatResponse } from '../models/ai-inventory-chat-response';

@Injectable({
  providedIn: 'root'
})
export class AssistantService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = '/api/ai/inventory/chat';

  ask(question: string): Observable<AiInventoryChatResponse> {
    const request: AiInventoryChatRequest = {
      question
    };

    return this.http.post<AiInventoryChatResponse>(
      this.apiUrl,
      request
    );
  }
}