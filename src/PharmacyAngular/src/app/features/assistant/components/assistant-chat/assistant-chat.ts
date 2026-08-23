import {
  AfterViewChecked,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  ViewChild,
  inject
} from '@angular/core';

import { AssistantInputComponent } from '../assistant-input/assistant-input';
import { AssistantMessageComponent } from '../assistant-message/assistant-message';

import { AssistantMessage } from '../../models/assistant-message';
import { AssistantService } from '../../services/assistant.service';

@Component({
  selector: 'app-assistant-chat',
  standalone: true,
  imports: [
    AssistantInputComponent,
    AssistantMessageComponent
  ],
  templateUrl: './assistant-chat.html',
  styleUrl: './assistant-chat.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AssistantChatComponent implements AfterViewChecked {

  private readonly assistantService = inject(AssistantService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  @ViewChild('messagesContainer')
  private messagesContainer?: ElementRef<HTMLDivElement>;

  messages: AssistantMessage[] = [];

  isLoading = false;

  errorMessage = '';

  private shouldScroll = false;

  constructor() {
    this.addWelcomeMessage();
  }

  ngAfterViewChecked(): void {
    if (this.shouldScroll) {
      this.scrollToBottom();
      this.shouldScroll = false;
    }
  }

  sendMessage(question: string): void {
    if (this.isLoading) {
      return;
    }

    this.errorMessage = '';

    this.messages = [
      ...this.messages,
      this.createMessage('user', question)
    ];

    this.isLoading = true;
    this.shouldScroll = true;

    this.assistantService.ask(question).subscribe({
      next: response => {
        this.messages = [
          ...this.messages,
          this.createMessage('assistant', response.answer)
        ];

        this.isLoading = false;
        this.shouldScroll = true;

        this.changeDetectorRef.markForCheck();
      },

      error: error => {
        console.error('AI assistant request failed.', error);

        this.errorMessage =
          'The assistant could not process your request. Please try again.';

        this.isLoading = false;
        this.shouldScroll = true;

        this.changeDetectorRef.markForCheck();
      }
    });
  }

  clearConversation(): void {
    this.messages = [];
    this.errorMessage = '';
    this.addWelcomeMessage();

    this.shouldScroll = true;
  }

  private addWelcomeMessage(): void {
    this.messages.push(
      this.createMessage(
        'assistant',
        'Hello! I can help you analyze the pharmacy inventory. You can ask me about stock levels, products, categories, and upcoming expirations.'
      )
    );
  }

  private createMessage(
    role: 'user' | 'assistant',
    content: string
  ): AssistantMessage {
    return {
      id: crypto.randomUUID(),
      role,
      content,
      createdAt: new Date()
    };
  }

  private scrollToBottom(): void {
    const element = this.messagesContainer?.nativeElement;

    if (!element) {
      return;
    }

    element.scrollTop = element.scrollHeight;
  }
}